import unittest
import numpy as np
from train import generate, split, export
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.linear_model import LogisticRegression

class TrainingTests(unittest.TestCase):
    def test_split_is_chronological_and_labels_mature(self):
        x = np.arange(4000).reshape(-1, 1)
        data = split(x, np.zeros(4000))
        self.assertLess(data['train'][0][-1, 0] + 7, data['validation'][0][0, 0])
        self.assertLess(data['validation'][0][-1, 0] + 7, data['test'][0][0, 0])
        self.assertEqual([len(v[1]) for v in data.values()], [2793, 593, 600])

    def test_export_matches_sklearn_probability(self):
        x, y = generate(count=300)
        model = make_pipeline(StandardScaler(), LogisticRegression()).fit(x, y)
        artifact = export(model)
        z = ((x - artifact['mean']) / artifact['scale']) @ np.array(artifact['weights']) + artifact['intercept']
        np.testing.assert_allclose(1 / (1 + np.exp(-z)), model.predict_proba(x)[:, 1], atol=1e-12)

if __name__ == '__main__':
    unittest.main()
