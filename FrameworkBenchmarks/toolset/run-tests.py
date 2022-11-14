import argparse
import socket
import sys
import signal
import traceback

from toolset.benchmark.benchmarker import Benchmarker
from toolset.utils.benchmark_config import BenchmarkConfig
from toolset.utils.output_helper import log

# Enable cross-platform colored output
from colorama import init, Fore
init()





###################################################################################################
# Main
###################################################################################################
def main():
    '''
    Runs the toolset.
    '''
    ##########################################################
    # Set up argument parser
    ##########################################################
    parser = argparse.ArgumentParser(formatter_class=argparse.ArgumentDefaultsHelpFormatter)

    # Test options
    parser.add_argument(
        '--test', default=None, nargs='+', help='names of tests to run')

    parser.add_argument(
        '--type', choices=['all', 'json', 'db', 'query', 'plaintext'],
        nargs='+', default='all', help='which type of test to run')

    # help
    parser.add_argument('--list-tests', action='store_true', default=False,
                        help='lists all the known tests that can run')

    args = parser.parse_args()
    config = BenchmarkConfig(args)
    benchmarker = Benchmarker(config)

    signal.signal(signal.SIGTERM, benchmarker.stop)
    signal.signal(signal.SIGINT, benchmarker.stop)

    try:
        # help
        if config.list_tests:
            all_tests = benchmarker.metadata.gather_tests()
            for test in all_tests:
                log(test.name)

        else:
            any_failed = benchmarker.run()

    except Exception:
        tb = traceback.format_exc()
        log("A fatal error has occurred", color=Fore.RED)
        log(tb)
        # try one last time to stop docker containers on fatal error
        try:
            benchmarker.stop()
        except:
            sys.exit(1)

    return 0


if __name__ == "__main__":
    sys.exit(main())
