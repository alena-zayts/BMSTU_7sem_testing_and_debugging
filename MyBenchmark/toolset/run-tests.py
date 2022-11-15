import argparse
import sys
import signal
import traceback

from toolset.benchmark.benchmarker import Benchmarker
from toolset.utils.benchmark_config import BenchmarkConfig
from toolset.utils.output_helper import log

# Enable cross-platform colored output
from colorama import init, Fore
init()


def main():
    parser = argparse.ArgumentParser(formatter_class=argparse.ArgumentDefaultsHelpFormatter)
    parser.add_argument(
        '--test', default=None, nargs='+', help='names of frameworks to run')

    parser.add_argument(
        '--type', choices=['all', 'json', 'db', 'query', 'plaintext'],
        nargs='+', default='all', help='which type of test to run')

    args = parser.parse_args()
    config = BenchmarkConfig(args)
    benchmarker = Benchmarker(config)

    signal.signal(signal.SIGTERM, benchmarker.stop)
    signal.signal(signal.SIGINT, benchmarker.stop)

    try:
        any_failed = benchmarker.run()

    except Exception:
        tb = traceback.format_exc()
        log("A fatal error has occurred", color=Fore.RED)
        log(tb)

        try:
            benchmarker.stop()
        except:
            sys.exit(1)

    return 0


if __name__ == "__main__":
    sys.exit(main())
