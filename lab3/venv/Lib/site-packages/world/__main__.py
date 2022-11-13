"""world script main entry point."""

import sys
import argparse

# We can drop List once Python 3.9 is the minimum supported version.
from typing import List  # Use list in Python >= 3.9
from typing import Optional

from public import public

from world import __version__
from world.database import Database


@public
def main(args: Optional[List[str]] = None) -> int:
    parser = argparse.ArgumentParser(
        prog='world', description='Top level domain name mapper.'
    )
    parser.add_argument(
        '--version', action='version', version='world {}'.format(__version__)
    )
    parser.add_argument_group('Querying')
    parser.add_argument(
        '-r',
        '--reverse',
        action='store_true',
        help="""Do a reverse lookup.  In this mode, the
                        arguments can be any Python regular expression; these
                        are matched against all TLD descriptions (e.g. country
                        names) and a list of matches is printed.""",
    )
    parser.add_argument(
        '-a',
        '--all',
        action='store_true',
        help='Print the mapping of all top-level domains.',
    )
    parser.add_argument('domain', nargs='*')
    parsed_args = parser.parse_args(sys.argv[1:] if args is None else args)
    # Lookup.
    db = Database()
    if parsed_args.all:
        print('Country code top level domains:')
        for cc in sorted(db.ccTLDs):
            print(f'    {cc}: {db.ccTLDs[cc]}')
        # Print the empty string instead of an empty print call for Python 2
        # compatibility with the test suite.  Otherwise we get a stupid
        # TypeError when io.StringIO gets a (Python 2) str instead of unicode.
        print('')
        print('Additional top level domains:')
        for tld in sorted(db):
            print(f'    {tld:6}: {db.lookup_code(tld)}')
        return 0
    if len(parsed_args.domain) == 0:
        parser.print_help()
        return 0
    newline = False
    return_code = 0
    for domain in parsed_args.domain:
        if parsed_args.reverse:
            if newline:
                # Print the empty string instead of an empty print call for
                # Python 2 compatibility with the test suite.  Otherwise we get
                # a stupid TypeError when io.StringIO gets a (Python 2) str
                # instead of unicode.
                print('')
            matches = db.find_matches(domain)
            if len(matches) > 0:
                print(f'Matches for "{domain}":')
                for code, country in matches:
                    print(f'  {code:6}: {country}')
                newline = True
                continue
        else:
            found_country = db.lookup_code(domain)
            if found_country is not None:
                print(f'{domain} originates from {found_country}')
                continue
        print(f'Where in the world is {domain}?')
        return_code += 1
    # Success.
    return return_code


if __name__ == '__main__':                          # pragma: no cover
    sys.exit(main())
