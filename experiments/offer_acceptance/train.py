"""Reproducible educational experiment; synthetic scores are not business evidence."""
import csv
import json
from pathlib import Path
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import numpy as np
from sklearn.linear_model import LogisticRegression
from sklearn.tree import DecisionTreeClassifier
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.metrics import precision_score, recall_score, average_precision_score, log_loss

ROOT = Path(__file__).resolve().parents[2]
OUT = Path(__file__).resolve().parent / 'results'
FEATURES = ['budget', 'planPrice', 'addOnPrice', 'priceToBudgetRatio']


def generate(seed=42, count=4000):
    rng = np.random.default_rng(seed)
    budget = rng.uniform(150000, 1500000, count)
    plan = budget * rng.uniform(.15, .75, count)
    addon = budget * rng.uniform(.02, .24, count)
    ratio = (plan + addon) / budget
    x = np.column_stack([budget, plan, addon, ratio])
    probability = 1 / (1 + np.exp(-(-.7 - 4 * (ratio - .55) + .0000003 * budget)))
    y = rng.binomial(1, probability)
    return x, y


def split(x, y):
    # Rows represent daily offers to distinct synthetic customers. A seven-day
    # embargo means all training labels mature before validation begins.
    n = len(y)
    a, b = int(n * .70), int(n * .85)
    return {'train': (x[:a-7], y[:a-7]), 'validation': (x[a:b-7], y[a:b-7]),
            'test': (x[b:], y[b:])}


def metrics(y, p):
    return {'precision': float(precision_score(y, p >= .5, zero_division=0)),
            'recall': float(recall_score(y, p >= .5, zero_division=0)),
            'prAuc': float(average_precision_score(y, p)),
            'logLoss': float(log_loss(y, p, labels=[0, 1]))}


def export(model):
    if isinstance(model, DecisionTreeClassifier):
        t = model.tree_
        def node(i):
            if t.children_left[i] == -1:
                return {'probability': float(t.value[i][0][1] / t.value[i][0].sum())}
            return {'feature': int(t.feature[i]), 'threshold': float(t.threshold[i]),
                    'left': node(t.children_left[i]), 'right': node(t.children_right[i])}
        return {'kind': 'tree', 'root': node(0)}
    scaler, lr = model.steps[0][1], model.steps[1][1]
    return {'kind': 'logistic', 'mean': scaler.mean_.tolist(), 'scale': scaler.scale_.tolist(),
            'weights': lr.coef_[0].tolist(), 'intercept': float(lr.intercept_[0])}


def main():
    OUT.mkdir(exist_ok=True)
    x, y = generate()
    data = split(x, y)
    from datetime import date, timedelta
    with (OUT / 'synthetic_offers.csv').open('w') as f:
        w = csv.writer(f)
        w.writerow(['customerId', 'offeredAt', 'labelObservedAt', *FEATURES, 'acceptedWithin7Days'])
        for i, (row, label) in enumerate(zip(x, y)):
            day = date(2010, 1, 1) + timedelta(days=i)
            w.writerow([f'synthetic-{i}', day, day + timedelta(days=7), *row, label])
    candidates = [(f'logistic_C_{c}', make_pipeline(StandardScaler(), LogisticRegression(C=c, random_state=42)))
                  for c in [.01, .1, 1, 10]]
    candidates += [(f'tree_depth_{depth}', DecisionTreeClassifier(max_depth=depth, random_state=42))
                   for depth in [1, 2, 3, 5, 8, 12, None]]
    rows, fitted = [], {}
    for name, model in candidates:
        model.fit(*data['train'])
        fitted[name] = model
        rows.append({'name': name, **{part: metrics(ys, model.predict_proba(xs)[:, 1])
                                     for part, (xs, ys) in data.items() if part != 'test'}})
    # Test is accessed only after choosing by validation log loss.
    best = min(rows, key=lambda r: r['validation']['logLoss'])
    model = fitted[best['name']]
    test = metrics(data['test'][1], model.predict_proba(data['test'][0])[:, 1])
    baseline = metrics(data['test'][1], np.full(len(data['test'][1]), data['train'][1].mean()))
    artifact = {'schemaVersion': 1, 'syntheticOnly': True, 'features': FEATURES,
                'selectedCandidate': best['name'], **export(model)}
    artifact_path = ROOT / 'Infrastructure/Offers/Models/offer-acceptance.json'
    artifact_path.parent.mkdir(parents=True, exist_ok=True)
    artifact_path.write_text(json.dumps(artifact, indent=2) + '\n')
    report = {'seed': 42, 'splitCounts': {k: len(v[1]) for k, v in data.items()},
              'embargoRows': 14, 'selectionMetric': 'validation log loss',
              'candidates': rows, 'selected': best['name'], 'test': test, 'constantBaselineTest': baseline}
    (OUT / 'metrics.json').write_text(json.dumps(report, indent=2) + '\n')
    trees = [r for r in rows if r['name'].startswith('tree')]
    for part in ['train', 'validation']:
        plt.plot(range(len(trees)), [r[part]['logLoss'] for r in trees], marker='o', label=part)
    plt.xticks(range(len(trees)), ['1', '2', '3', '5', '8', '12', 'unlimited'])
    plt.xlabel('Decision tree maximum depth'); plt.ylabel('Log loss (lower is better)')
    plt.title('Synthetic offer acceptance: overfitting'); plt.legend(); plt.tight_layout()
    plt.savefig(OUT / 'overfitting.png'); plt.close()
    lines = ['# Synthetic experiment results', '', 'Educational data only; no production efficacy claim.', '',
             '| Candidate | Train log loss | Validation log loss |', '|---|---:|---:|']
    lines += [f"| {r['name']} | {r['train']['logLoss']:.4f} | {r['validation']['logLoss']:.4f} |" for r in rows]
    lines += ['', f"Selected on validation: **{best['name']}**.", '',
              f"Held-out test: {json.dumps(test)}", '', f"Constant baseline test: {json.dumps(baseline)}", '',
              '![Overfitting by tree depth](overfitting.png)', '',
              'A deeper tree can memorize training outcomes while validation loss rises. Choose complexity using validation, never test.']
    (OUT / 'REPORT.md').write_text('\n'.join(lines) + '\n')
    print(json.dumps({'selected': best['name'], 'test': test}, indent=2))


if __name__ == '__main__':
    main()
