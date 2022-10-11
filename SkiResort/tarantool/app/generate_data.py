import tarantool
import traceback
import json
import random
from random import randint, choice, sample
from collections import defaultdict
from faker import Faker
import datetime
import faker.providers.date_time as fake_dt
import calendar
import os
import shutil

random.seed(0)
Faker.seed(0)
fake = Faker()


class Table:
    json_filename = None
    space_name = None

    def to_json(self):
        return self.__dict__

    def __repr__(self) -> str:
        return str(self)

    def __str__(self) -> str:
        result = ''
        for key, value in self.__dict__.items():
            result += f'{key}={value}, '
        return result

    @classmethod
    def generate_data(cls):
        return []



PERMISSIONS = {'admin': 3,
               'ski_patrol': 2,
               'authorized_user': 1,
               'unauthorized_user': 0
               }


class User(Table):
    json_filename = "json_data/users.json"
    space_name = 'users'

    def __init__(self, user_id, card_id, user_email, password, permissions):
        self.user_id = user_id
        self.card_id = card_id if card_id else 0
        self.user_email = user_email
        self.password = password
        self.permissions = permissions

    @classmethod
    def generate_data(cls, n_unauthorized=490, n_authorized=500, n_ski_patrol=10):
        data = []

        data.append(User(1, 0, 'q', 'q',
                         PERMISSIONS['admin']).to_json())

        for cur_id in range(n_unauthorized):
            data.append(User(cur_id + 2, 0, f"a{cur_id + 2}", f"a{cur_id + 2}", PERMISSIONS['unauthorized_user']).to_json())

        for cur_id in range(n_authorized):
            data.append(User(cur_id + 2 + n_unauthorized, 0,
                             f'authorized_email{cur_id}', f'authorized_password{cur_id}',
                             PERMISSIONS['authorized_user']).to_json())

        for cur_id in range(n_ski_patrol):
            data.append(User(cur_id + 2 + n_unauthorized + n_authorized, 0,
                             f'ski_patrol_email{cur_id}',
                             f'ski_patrol_password{cur_id}',
                             PERMISSIONS['ski_patrol']).to_json())

        return data


class Slope(Table):
    json_filename = "json_data/slopes.json"
    space_name = 'slopes'

    def __init__(self, slope_id, slope_name, difficulty_level, is_open):
        self.slope_id = slope_id
        self.slope_name = slope_name
        self.is_open = is_open
        self.difficulty_level = difficulty_level

    @classmethod
    def generate_data(cls, n_slopes_bunches=10, slopes_per_bunch=(8, 11), difficulty_levels=(1, 10)):
        data = []
        alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        slope_id = 1
        for letter in alphabet[:n_slopes_bunches]:
            for i in range(randint(*slopes_per_bunch)):
                data.append(Slope(slope_id, f'{letter}{i}',
                                  randint(*difficulty_levels), choice([True, False])).to_json())
                slope_id += 1

        return data


class Lift(Table):
    json_filename = "json_data/lifts.json"
    space_name = 'lifts'

    def __init__(self, lift_id, lift_name, is_open, seats_amount, lifting_time, queue_time=0):
        self.lift_id = lift_id
        self.lift_name = lift_name
        self.is_open = is_open
        self.seats_amount = seats_amount
        self.lifting_time = lifting_time  # sec
        self.queue_time = queue_time if queue_time != 0 else randint(15, 100)  # sec

    @classmethod
    def generate_data(cls,
                      n_lifts_bunches=10, lifts_per_bunch=(8, 11),
                      #n_lifts_bunches=1, lifts_per_bunch=(1, 1),
                      lifting_times=(30, 300), seats_amounts=(10, 100)):
        data = []
        alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        slope_id = 1
        for letter in alphabet[:n_lifts_bunches]:
            for i in range(randint(*lifts_per_bunch)):
                data.append(Lift(slope_id, f'{letter}{i}', choice([True, False]),
                                 randint(*seats_amounts), randint(*lifting_times)).to_json())
                slope_id += 1

        return data


class LiftSlope(Table):
    json_filename = "json_data/lifts_slopes.json"
    space_name = 'lifts_slopes'

    def __init__(self, record_id, lift_id, slope_id):
        self.record_id = record_id
        self.lift_id = lift_id
        self.slope_id = slope_id

    @classmethod
    def generate_data(cls):
        with open(Lift.json_filename, 'r') as f:
            lifts = [Lift(*(list(lift_dict.values())[:-1])) for lift_dict in json.load(f)]

        grouped_lifts = defaultdict(list)
        for lift in lifts:
            grouped_lifts[lift.lift_name[0]].append(lift)

        with open(Slope.json_filename, 'r') as f:
            slopes = [Slope(*slope_dict.values()) for slope_dict in json.load(f)]

        grouped_slopes = defaultdict(list)
        for slope in slopes:
            grouped_slopes[slope.slope_name[0]].append(slope)

        data = []

        i = 1
        for lift_letter, lifts in grouped_lifts.items():
            first_lift = lifts[0]
            slopes_for_letter = grouped_slopes[lift_letter]

            # первый со всеми
            for slope in slopes_for_letter:
                data.append(LiftSlope(i, first_lift.lift_id, slope.slope_id).to_json())
                i += 1

            # остальные со случайным числом остальных
            if len(lifts) > 1:
                for lift in lifts[1:]:
                    n_slopes_to_connect = randint(0, len(slopes_for_letter) - 1)
                    slopes_to_connect = sample(slopes_for_letter, n_slopes_to_connect)
                    for slope in slopes_to_connect:
                        data.append(LiftSlope(i, lift.lift_id, slope.slope_id).to_json())
                        i += 1

        return data


class Turnstile(Table):
    json_filename = "json_data/turnstiles.json"
    space_name = 'turnstiles'

    def __init__(self, turnstile_id, lift_id, is_open):
        self.turnstile_id = turnstile_id
        self.lift_id = lift_id
        self.is_open = is_open

    @classmethod
    def generate_data(cls, turnstiles_per_lift=(1, 5), lifts=None):
        if lifts is None:
            with open(Lift.json_filename, 'r') as f:
                lifts = [Lift(*lift_dict.values()) for lift_dict in json.load(f)]

        data = []

        i = 0
        for lift in lifts:
            n_turnstiles = randint(*turnstiles_per_lift)
            for _ in range(n_turnstiles):
                try:
                    data.append(Turnstile(i + 1, lift.lift_id, choice([True, False])).to_json())
                except:
                    data.append(Turnstile(i + 1, lift['lift_id'], choice([True, False])).to_json())
                i += 1

        return data


class Card(Table):
    json_filename = "json_data/cards.json"
    space_name = 'cards'

    def __init__(self, card_id, activation_time, card_type):
        self.card_id = card_id
        self.activation_time = activation_time
        self.type = card_type

    @classmethod
    def generate_data(cls, n_cards=1000, date_limits=(1585575897, 1648647930),  # 30.03.20-30.03.2022
                      types=['child', 'adult']):
        data = []

        for i in range(n_cards):
            data.append(Card(i + 1, randint(*date_limits), choice(types)).to_json())

        return data


class CardReading(Table):
    json_filename = "json_data/card_readings.json"
    space_name = 'card_readings'

    def __init__(self, record_id, turnstile_id, card_id, reading_time):
        self.record_id = record_id
        self.turnstile_id = turnstile_id
        self.card_id = card_id
        self.reading_time = reading_time

    @classmethod
    def generate_data(cls, n_readings=10000, date_limits=(1585575897, 1648647930),  # 30.03.20-30.03.2022
                      ):
        with open(Turnstile.json_filename, 'r') as f:
            turnstiles = [Turnstile(*(list(turnstile_dict.values()))) for turnstile_dict in json.load(f)]
        turnstile_ids = [turnstile.turnstile_id for turnstile in turnstiles]

        with open(Card.json_filename, 'r') as f:
            cards = [Card(*(list(card_dict.values()))) for card_dict in json.load(f)]
        card_ids = [card.card_id for card in cards]

        data = []

        for i in range(n_readings):
            data.append(CardReading(i + 1, choice(turnstile_ids), choice(card_ids), randint(*date_limits)).to_json())

        return data



class Message(Table):
    json_filename = "json_data/messages.json"
    space_name = 'messages'

    def __init__(self, message_id, sender_id, checked_by_id, text):
        self.message_id = message_id
        self.sender_id = sender_id
        self.checked_by_id = checked_by_id
        self.text = text

    @classmethod
    def generate_data(cls, n=1000):
        with open(User.json_filename, 'r') as f:
            users = [User(*(list(user_dict.values()))) for user_dict in json.load(f)]

        sender_ids = [user.user_id for user in users if user.permissions == PERMISSIONS['unauthorized_user']]
        checked_by_ids = [user.user_id for user in users if user.permissions == PERMISSIONS['ski_patrol']]
        checked_by_ids.append(0)


        data = []

        for i in range(n):
            data.append(Message(i + 1, choice(sender_ids), choice(checked_by_ids), f"text{i+1}").to_json())

        return data


def generate_table_data_to_json_file(table: Table):
    with open(table.json_filename, "w") as write_file:
        json.dump(table.generate_data(), write_file)




def generate_all_data_to_json_file():
    # generate_table_data_to_json_file(UsersGroup)
    generate_table_data_to_json_file(User)
    generate_table_data_to_json_file(Slope)
    generate_table_data_to_json_file(Lift)
    generate_table_data_to_json_file(LiftSlope)
    generate_table_data_to_json_file(Turnstile)
    generate_table_data_to_json_file(Card)
    generate_table_data_to_json_file(CardReading)
    generate_table_data_to_json_file(Message)







def generate_card_readings(dir_name, n, date_limits=(1652659200, 1652745600)): # 16-17 мая
    import time
    import datetime
    global_dir = "C:/BMSTU_7sem_web/src/tarantool/app/json_data/card_readings/"
    if not os.path.isdir(f"{global_dir}{dir_name}"):
        os.mkdir(f"{global_dir}{dir_name}")

    for i in range(n):
        cur_time = time.mktime(datetime.datetime.now().timetuple())
        obj = CardReading2.generate(date_limits=date_limits)

        file_name = f"{global_dir}{dir_name}card_reading_{i}.json"

        with open(file_name, "w") as write_file:
            json.dump(obj, write_file)
        if i % 1000 == 0:
            print(file_name, cur_time)
    #print()

def generate_tests():
    for n in range(1000, 10000, 1000):
        generate_card_readings(f"n_{n}/", n)




def generate_lifts(n):
    with open(Lift.json_filename, 'r') as f:
        lifts = [Lift(*lift_dict.values()) for lift_dict in json.load(f)]

    lifts = lifts[:n]


    global_dir = "C:/BMSTU_7sem_web/src/tarantool/app/json_data/lifts/"
    if not os.path.isdir(f"{global_dir}"):
        os.mkdir(f"{global_dir}")

    for i in range(n):
        obj = lifts[i].to_json()

        file_name = f"{global_dir}lift_{i}.json"

        with open(file_name, "w") as write_file:
            json.dump(obj, write_file)
        if i % 1000 == 0:
            print(file_name)





work = False

def generate_test3():
    path = "C:/BMSTU_7sem_web/src/settings.txt"
    if work:
        path = "/home/alex/tmp/BMSTU_7sem_web/src/settings.txt"
    with open(path, 'r') as f:
        date_from = int(f.readline())
        date_to = int(f.readline())
        n_lifts = int(f.readline())
        n_turnstiles_per_lift = int(f.readline())
        n_cardreadings_per_turnstile = int(f.readline())

    global_lift_dir = "C:/BMSTU_7sem_web/src/tarantool/app/json_data/lifts/"
    if work:
        global_lift_dir = "/home/alex/tmp/BMSTU_7sem_web/src/tarantool/app/json_data/lifts/"
    global_turnstiles_dir = "C:/BMSTU_7sem_web/src/tarantool/app/json_data/turnstiles/"
    if work:
        global_turnstiles_dir = "/home/alex/tmp/BMSTU_7sem_web/src/tarantool/app/json_data/turnstiles/"
    global_card_readings_dir = "C:/BMSTU_7sem_web/src/tarantool/app/json_data/card_readings/"
    if work:
        global_card_readings_dir = "/home/alex/tmp/BMSTU_7sem_web/src/tarantool/app/json_data/card_readings/"

    if os.path.isdir(f"{global_lift_dir}"):
        shutil.rmtree(f"{global_lift_dir}")
    os.mkdir(f"{global_lift_dir}")
    if os.path.isdir(f"{global_turnstiles_dir}"):
        shutil.rmtree(f"{global_turnstiles_dir}")
    os.mkdir(f"{global_turnstiles_dir}")
    if os.path.isdir(f"{global_card_readings_dir}"):
        shutil.rmtree(f"{global_card_readings_dir}")
    os.mkdir(f"{global_card_readings_dir}")

    lifts = Lift.generate_data(n_lifts_bunches=10, lifts_per_bunch=(n_lifts // 10, n_lifts // 10))
    turnstiles = Turnstile.generate_data(turnstiles_per_lift=(n_turnstiles_per_lift, n_turnstiles_per_lift), lifts=lifts)

    for i in range(len(lifts)):
        obj = lifts[i]
        file_name = f"{global_lift_dir}lift_{i}.json"
        with open(file_name, "w") as write_file:
            json.dump(obj, write_file)

    for i in range(len(turnstiles)):
        obj = turnstiles[i]
        file_name = f"{global_turnstiles_dir}turnstile_{i}.json"
        with open(file_name, "w") as write_file:
            json.dump(obj, write_file)

    for i in range(len(turnstiles)):
        for j in range(n_cardreadings_per_turnstile):
            obj = CardReading2.generate(date_limits=(date_from, date_to), turnstile_id=turnstiles[i]['turnstile_id'])

            file_name = f"{global_card_readings_dir}card_reading_{i * n_cardreadings_per_turnstile + j}.json"

            with open(file_name, "w") as write_file:
                json.dump(obj, write_file)

        if i % 100 == 0:
            print(f"{i} turnstile")


class CardReading2(Table):
    def __init__(self, record_id, turnstile_id, card_id, reading_time):
        self.RecordID = record_id
        self.TurnstileID = turnstile_id
        self.CardID = card_id
        self.ReadingTime = reading_time

    @classmethod
    def generate(cls, date_limits=(1585575897, 1648647930), turnstile_id=None):
        with open(Turnstile.json_filename, 'r') as f:
            turnstiles = [Turnstile(*(list(turnstile_dict.values()))) for turnstile_dict in json.load(f)]
        turnstile_ids = [turnstile.turnstile_id for turnstile in turnstiles]

        with open(Card.json_filename, 'r') as f:
            cards = [Card(*(list(card_dict.values()))) for card_dict in json.load(f)]
        card_ids = [card.card_id for card in cards]

        return CardReading2(0, choice(turnstile_ids) if not turnstile_id else turnstile_id, choice(card_ids), randint(*date_limits)).to_json()


def infinite_card_readings_generator(sleep_time=1, n_in_time=1):
    import time
    import datetime

    i = 0

    while True:
        for _ in range(n_in_time):
            cur_time = time.mktime(datetime.datetime.now().timetuple())
            obj = CardReading2.generate(date_limits=(cur_time - sleep_time, cur_time))

            file_name = "C:/BMSTU_7sem_web/src/tarantool/app/json_data/card_readings/card_reading_" + str(i) + ".json"

            with open(file_name, "w") as write_file:
                json.dump(obj, write_file)

            print(file_name, cur_time)

            i += 1
        time.sleep(sleep_time)

import psycopg2
def main2():
    # Подключаемся к БД.
    try:
        con = psycopg2.connect(
            database="postgres",
            user="postgres",
            password="4541",
            host="127.0.0.1",  # Адрес сервера базы данных.
            port="5432"		   # Номер порта.
        )
    except:
        print("Ошибка при подключении к БД")
        return

    print("База данных успешно открыта")

    # Объект cursor используется для фактического
    # выполнения наших команд.
    cur = con.cursor()



    # Закрываем соединение с БД.
    cur.close()
    con.close()

if __name__ == "__main__":
    generate_all_data_to_json_file()
    # infinite_card_readings_generator(sleep_time=1, n_in_time=20)  # sec
    #generate_one_test("", 10000)
    # generate_lifts(1000)
    # generate_test3()






