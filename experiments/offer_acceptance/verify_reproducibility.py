"""Verify retraining with numerical tolerance for BLAS/platform roundoff."""
import json
import math
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
PATHS = ['Infrastructure/Offers/Models/offer-acceptance.json',
         'experiments/offer_acceptance/results/metrics.json']


def compare(expected, actual, path='root'):
    if isinstance(expected, bool):
        assert expected is actual, path
    elif isinstance(expected, (float, int)):
        assert math.isclose(expected, actual, rel_tol=1e-10, abs_tol=1e-12), path
    elif isinstance(expected, dict):
        assert expected.keys() == actual.keys(), path
        for key in expected:
            compare(expected[key], actual[key], f'{path}.{key}')
    elif isinstance(expected, list):
        assert len(expected) == len(actual), path
        for index, (left, right) in enumerate(zip(expected, actual)):
            compare(left, right, f'{path}[{index}]')
    else:
        assert expected == actual, path


if __name__ == '__main__':
    expected = {path: json.loads((ROOT / path).read_text()) for path in PATHS}
    subprocess.run([sys.executable, str(Path(__file__).with_name('train.py'))], check=True)
    for path in PATHS:
        compare(expected[path], json.loads((ROOT / path).read_text()), path)
    print('Artifact and report reproduced within numerical tolerance.')
