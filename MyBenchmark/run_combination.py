import os

N_REPEATS = 5
smth_failed = False


def main():
    for i in range(N_REPEATS):
        print(f'\n\n\n\n\n\n\nTEST CRAX: {i}\n\n\n\n\n\n\n')
        if os.system('./bm --test crax') != 0:
            smth_failed = True

    for i in range(N_REPEATS):
        print(f'\n\n\n\n\n\n\nTEST AIOHTTP: {i}\n\n\n\n\n\n\n')
        if os.system('./bm --test aiohttp') != 0:
            smth_failed = True


if __name__ == '__main__':
    input('sure?')
    main()
