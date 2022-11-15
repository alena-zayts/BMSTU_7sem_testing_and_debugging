import abc
import copy
import requests

from colorama import Fore
from toolset.utils.output_helper import log

class AbstractTestType:
    '''
    Interface between a test type (json, query, plaintext, etc) and
    the rest of TFB. A test type defines a number of keys it expects
    to find in the benchmark_config.json, and this base class handles extracting
    those keys and injecting them into the test. For example, if
    benchmark_config.json contains a line `"spam" : "foobar"` and a subclasses X
    passes an argument list of ['spam'], then after parsing there will
    exist a member `X.spam = 'foobar'`.
    '''
    __metaclass__ = abc.ABCMeta

    def __init__(self, config, name, requires_db=False, accept_header=None, args=[]):
        self.config = config
        self.name = name
        self.requires_db = requires_db
        self.args = args
        self.headers = ""
        self.body = ""

        if accept_header is None:
            self.accept_header = self.what_accepts('json')
        else:
            self.accept_header = accept_header

        self.passed = None
        self.failed = None
        self.warned = None

    @classmethod
    @abc.abstractmethod
    def what_accepts(cls, content_type):
        return {
            'json': 'application/json,text/html;q=0.9,application/xhtml+xml;q=0.9,application/xml;q=0.8,*/*;q=0.7',
            'plaintext': 'text/plain,text/html;q=0.9,application/xhtml+xml;q=0.9,application/xml;q=0.8,*/*;q=0.7'
        }[content_type]

    def parse(self, test_keys):
        """
        Takes the dict of key/value pairs describing a FrameworkTest and collects all variables needed
        """
        if all(arg in test_keys for arg in self.args):
            self.__dict__.update({arg: test_keys[arg] for arg in self.args})
            return self
        else:  # This is quite common - most tests don't support all types
            raise AttributeError("A %s requires the benchmark_config.json to contain %s" % (self.name, self.args))

    def request_headers_and_body(self, url):
        """
        Downloads a URL and returns the HTTP response headers and body content as a tuple
        """
        log("Accessing URL {!s}: ".format(url), color=Fore.CYAN)

        headers = {'Accept': self.accept_header}
        r = requests.get(url, timeout=15, headers=headers)

        self.headers = r.headers
        self.body = r.content
        return self.headers, self.body

    def get_url(self):
        raise NotImplementedError()

    def get_script_name(self):
        raise NotImplementedError()

    def get_script_variables(self, name, url):
        raise NotImplementedError()
