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


class StoreSeqAction(argparse.Action):
    '''
    Helper class for parsing a sequence from the command line
    '''

    def __init__(self, option_strings, dest, nargs=None, **kwargs):
        super(StoreSeqAction, self).__init__(
            option_strings, dest, type=str, **kwargs)

    def __call__(self, parser, namespace, values, option_string=None):
        setattr(namespace, self.dest, self.parse_seq(values))

    def parse_seq(self, argument):
        result = argument.split(',')
        sequences = [x for x in result if ":" in x]
        for sequence in sequences:
            try:
                (start, step, end) = sequence.split(':')
            except ValueError:
                log("  Invalid: {!s}".format(sequence), color=Fore.RED)
                log("  Requires start:step:end, e.g. 1:2:10", color=Fore.RED)
                raise
            result.remove(sequence)
            result = result + range(int(start), int(end), int(step))
        return [abs(int(item)) for item in result]


###################################################################################################
# Main
###################################################################################################
def main(argv=None):
    '''
    Runs the toolset.
    '''
    ##########################################################
    # Set up argument parser
    ##########################################################
    parser = argparse.ArgumentParser(
        description="Install or run the Framework Benchmarks test suite.",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter)

    # unused but difficult
    parser.add_argument(
        '--results-environment',
        help='Describes the environment in which these results were gathered',
        default='(unspecified, hostname = %s)' % socket.gethostname())

    # Test options
    parser.add_argument(
        '--test', default=None, nargs='+', help='names of tests to run')

    parser.add_argument(
        '--type',
        choices=[
            'all', 'json', 'db', 'query', 'plaintext'
        ],
        nargs='+',
        default='all',
        help='which type of test to run')

    # help
    parser.add_argument(
        '--list-tests',
        action='store_true',
        default=False,
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
