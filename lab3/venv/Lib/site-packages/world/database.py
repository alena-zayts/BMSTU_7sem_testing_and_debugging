"""The database and lookups."""

import re
import pickle

from importlib.resources import open_binary
from typing import Dict  # Use dict in Python >= 3.9
from typing import List  # Use list in Python >= 3.9
from typing import Generator, Optional, Tuple

from public import public


@public
class Database:
    """Database of country codes to top-level domain names."""

    def __init__(self) -> None:
        with open_binary('world.data', 'codes.pck') as fp:
            raw_data = pickle.load(fp)
        # We want two lookup tables.  One maps English country names to
        # 2-digit alpha country codes.  Another maps the 2-digit codes to
        # their English names.  For now, while we capture the 3-digit alpha
        # codes in the raw data, we ignore it.
        self.ccTLDs: Dict[str, str] = {
            code2.lower(): english for english, code2, code3 in raw_data
        }
        # Some additional common mappings.
        self._by_code: Dict[str, str] = self.ccTLDs.copy()
        self._by_code.update(gTLDs)

    def lookup_code(self, domain: str) -> Optional[str]:
        """Given a top-level country code, return the country if possible.

        :param domain: The country code to look up.  The code is case
            insensitive.
        :return: The country name matching the given code.  If the given code
            does not match any entries in the database, None is returned.
        """
        return self._by_code.get(domain.lower())

    def find_matches(self, text: str) -> List[Tuple[str, str]]:
        """Find all reverse matches of a given text.

        ``text`` can be any Python regular expression.  This is matched
        against every value (i.e. country name, not code) in the database
        Every match is returned as a list of 2-tuples where the first element
        is the country code and the second element is the country name.

        :param text: Regular expression to match against.
        :return: The list of matching 2-tuples, of the form (code, name)
        """
        matches: List[Tuple[str, str]] = []
        cre = re.compile(text, re.IGNORECASE)
        for key, value in self._by_code.items():
            if cre.search(value):
                matches.append((key, value))
        return sorted(matches)

    def __iter__(self) -> Generator[str, None, None]:
        """Iterate over all country codes, in sorted order."""
        for code in sorted(self._by_code):
            yield code


# Generic top-level domains.
# http://en.wikipedia.org/wiki/TLD
#
# Of course, the internet has changed considerably in the intervening years
# since this tool was first written, and we now have a jillion new TLDs with
# more coming online every day.  Let's not even talk about non-Latin TLDs.  I
# don't care about any of those; if you do, fork me!
gTLDs = {
    # fmt: off
    # Intrastructure.
    'arpa': 'Arpanet',
    # Additional IANA TLDs.
    'aero': 'air-transport industry',
    'asia': 'Asia-Pacific region',
    'biz' : 'business',                             # noqa: E203
    'cat' : 'Catalan',                              # noqa: E203
    'com' : 'commercial',                           # noqa: E203
    'coop': 'cooperatives',
    'info': 'information',
    'int' : 'international organizations',          # noqa: E203
    'jobs': 'companies',
    'mobi': 'mobile devices',
    'museum': 'museums',
    'name': 'individuals, by name',
    'net' : 'network',                              # noqa: E203
    'org' : 'non-commercial',                       # noqa: E203
    'post': 'postal services',
    'pro' : 'professionals',                        # noqa: E203
    'tel' : 'Internet communications services',     # noqa: E203
    'travel': 'travel and tourism industry related sites',
    'xxx' : 'adult entertainment',                  # noqa: E203
    # USA TLDs.
    'edu' : 'educational',                          # noqa: E203
    'gov' : 'governmental',                         # noqa: E203
    'mil' : 'US military',                          # noqa: E203
    # These additional ccTLDs are included here even though they are not part
    # of ISO 3166.  IANA has 5 reserved ccTLDs as described here:
    #
    # http://www.iso.org/iso/en/prods-services/iso3166ma/04background-on-iso-3166/iso3166-1-and-ccTLDs.html
    #
    # but I can't find an official list anywhere.
    #
    # Note that `uk' is the common practice country code for the United
    # Kingdom.  AFAICT, the official `gb' code is routinely ignored!
    #
    # <D.M.Pick@qmw.ac.uk> tells me that `uk' was long in use before ISO3166
    # was adopted for top-level DNS zone names (although in the reverse order
    # like uk.ac.qmw) and was carried forward (with the reversal) to avoid a
    # large-scale renaming process as the UK switched from their old `Coloured
    # Book' protocols over X.25 to Internet protocols over IP.
    #
    # See <url:ftp://ftp.ripe.net/ripe/docs/ripe-159.txt>
    'ac': 'Ascension Island',
    'eu': 'European Union',
    'su': 'Soviet Union (historical)',
    'tp': 'East Timor (obsolete)',
    'uk': 'United Kingdom (common practice)',
    # fmt: on
}
