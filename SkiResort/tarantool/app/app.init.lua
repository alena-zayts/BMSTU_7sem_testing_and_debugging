#!/usr/bin/env tarantool


-- lsof -i :3301

io_module = require("io")
io_module.stdout:setvbuf("no")
math_module = require("math")

print('start!')



---------------------------------------------------------------------------------------------init tables
local function init()
	print('in init!')
	
	box.schema.upgrade()
	
	-- print(box.info.version)
	--box.schema.user.create('ski_admin', {if_not_exists = true}, {password = 'Tty454r293300'})
	--box.schema.user.passwd('ski_admin', 'Tty454r293300')
	--box.schema.user.grant('ski_admin', 'read,write,execute,create,alter,drop', 'universe')


	box.space.users:drop()
	box.space.cards:drop()
	box.space.card_readings:drop()
	box.space.turnstiles:drop()
	box.space.messages:drop()
	box.space.lifts:drop()
	box.space.slopes:drop()
	box.space.lifts_slopes:drop()

	
	

	--- users
	users = box.schema.space.create('users', {field_count=5, engine=chosen_engine})
	users:format({
		{name = 'user_id', type = 'unsigned'},
		{name = 'card_id', type = 'unsigned'},
		{name = 'user_email', type = 'string'},
		{name = 'password', type = 'string'},
		{name = 'permissions', type = 'unsigned'}
	})
	users:create_index('primary')
	users:create_index('index_email', {parts = {'user_email'}})
	print('users created!')

	--- cards
	cards = box.schema.space.create('cards', {field_count=3, engine=chosen_engine})
	cards:format({
		{name = 'card_id', type = 'unsigned'},
		{name = 'activation_time', type = 'unsigned'},
		{name = 'type', type = 'string'},
	})
	cards:create_index('primary')
	print('cards created!')

	--- card_readings
	card_readings = box.schema.space.create('card_readings', {field_count=4, engine=chosen_engine})
	card_readings:format({
		{name = 'record_id', type = 'unsigned'},
		{name = 'turnstile_id', type = 'unsigned'},
		{name = 'card_id', type = 'unsigned'},
		{name = 'reading_time', type = 'unsigned'},
	})
	card_readings:create_index('primary')
	card_readings:create_index('index_turnstile', {unique = false, parts = {'turnstile_id'}})
	card_readings:create_index('index_time', {unique = false, parts = {'reading_time'}})
	print('card_readings created!')

	--- turnstiles
	turnstiles = box.schema.space.create('turnstiles', {field_count=3, engine=chosen_engine})
	turnstiles:format({
		{name = 'turnstile_id', type = 'unsigned'},
		{name = 'lift_id', type = 'unsigned'},
		{name = 'is_open', type = 'boolean'}
	})
	turnstiles:create_index('primary')
	turnstiles:create_index('index_lift_id', {unique = false, parts = {'lift_id'}})
	print('turnstiles created!')


	--- messages
	messages = box.schema.space.create('messages', {field_count=4, engine=chosen_engine})
	messages:format({
		{name = 'message_id', type = 'unsigned'},
		{name = 'sender_id', type = 'unsigned'},
		{name = 'checked_by_id', type = 'unsigned'},
		{name = 'text', type = 'string'},
	})
	messages:create_index('primary')
	messages:create_index('index_sender_id', {unique = false, parts = {'sender_id'}})
	messages:create_index('index_checked_by_id', {unique = false, parts = {'checked_by_id'}})
	print('messages created!')


	--- lifts
	lifts = box.schema.space.create('lifts', {field_count=6, engine=chosen_engine})
	lifts:format({
		{name = 'lift_id', type = 'unsigned'},
		{name = 'lift_name', type = 'string'},
		{name = 'is_open', type = 'boolean'},
		{name = 'seats_amount', type = 'unsigned'},
		{name = 'lifting_time', type = 'unsigned'},
		{name = 'queue_time', type = 'unsigned'},
	})
	lifts:create_index('primary')
	lifts:create_index('index_name', {parts = {'lift_name'}})
	print('lifts created!')


	--- slopes
	slopes = box.schema.space.create('slopes', {field_count=4, engine=chosen_engine})
	slopes:format({
		{name = 'slope_id', type = 'unsigned'},
		{name = 'slope_name', type = 'string'},
		{name = 'is_open', type = 'boolean'},
		{name = 'difficulty_level', type = 'unsigned'}
	})
	slopes:create_index('primary')
	slopes:create_index('index_name', {parts = {'slope_name'}})
	print('slopes created!')


	--- lifts_slopes
	lifts_slopes = box.schema.space.create('lifts_slopes', {field_count=3, engine=chosen_engine})
	lifts_slopes:format({
		{name = 'record_id', type = 'unsigned'},
		{name = 'lift_id', type = 'unsigned'},
		{name = 'slope_id', type = 'unsigned'},
	})
	lifts_slopes:create_index('primary')
	lifts_slopes:create_index('index_lift_id', {unique = false, parts = {'lift_id'}})
	lifts_slopes:create_index('index_slope_id', {unique = false, parts = {'slope_id'}})
	lifts_slopes:create_index('index_lift_slope', {parts = {'lift_id', 'slope_id'}})
	print('lifts_slopes created!')
end






------------------------------------------------------------------------------------------------fill tables
json=require('json')
msgpack=require('msgpack')
json_data_dir = "/usr/local/share/tarantool/json_data/"


local function load_users_data()
    local cur_space = box.space.users
	local cur_filename = "users.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["user_id"], v["card_id"], v["user_email"], v["password"], v["permissions"]}
    end
end

local function load_cards_data()
    local cur_space = box.space.cards
	local cur_filename = "cards.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["card_id"], v["activation_time"], v["type"]}
    end
end

local function load_turnstiles_data()
    local cur_space = box.space.turnstiles
	local cur_filename = "turnstiles.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["turnstile_id"], v["lift_id"], v["is_open"]}
    end
end

local function load_lifts_data()
    local cur_space = box.space.lifts
	local cur_filename = "lifts.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["lift_id"], v["lift_name"], v["is_open"], v["seats_amount"], v["lifting_time"], v["queue_time"]}
    end
end


local function load_slopes_data()
    local cur_space = box.space.slopes
	local cur_filename = "slopes.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["slope_id"], v["slope_name"], v["is_open"], v["difficulty_level"]}
    end
end

local function load_lifts_slopes_data()
    local cur_space = box.space.lifts_slopes
	local cur_filename = "lifts_slopes.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["record_id"], v["lift_id"], v["slope_id"]}
    end
end

local function load_messages_data()
    local cur_space = box.space.messages
	local cur_filename = "messages.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["message_id"], v["sender_id"], v["checked_by_id"], v["text"]}
    end
end


local function load_card_readings_data()
    local cur_space = box.space.card_readings
	local cur_filename = "card_readings.json"

	local file = io.open(json_data_dir .. cur_filename, "r")
	a = file:read("*a")
	file:close()

	cur_table = json.decode(a)

	for k,v in pairs(cur_table) do
		cur_space:insert{v["record_id"], v["turnstile_id"], v["card_id"], v["reading_time"]}
    end
end


local function load__data()
	load_users_data()
	load_cards_data()
	load_turnstiles_data()
	load_lifts_data()
	load_slopes_data()
	load_lifts_slopes_data()
	load_messages_data()
	--load_card_readings_data()
end

----------------------------------------------------------------------------------------------------functions

function auto_increment_users(card_id, user_email, password, permissions)
	return box.space.users:auto_increment{card_id, user_email, password, permissions}
end

function auto_increment_cards(activation_time, card_type)
	return box.space.cards:auto_increment{activation_time, card_type}
end

function auto_increment_card_readings(turnstile_id, card_id, reading_time)
	return box.space.card_readings:auto_increment{turnstile_id, card_id, reading_time}
end

function auto_increment_turnstiles(lift_id, is_open)
	return box.space.turnstiles:auto_increment{lift_id, is_open}
end

function auto_increment_lifts(lift_name, is_open, seats_amount, lifting_time, queue_time)
	return box.space.lifts:auto_increment{lift_name, is_open, seats_amount, lifting_time, queue_time}
end

function auto_increment_slopes(slope_name, is_open, difficulty_level)
	return box.space.slopes:auto_increment{slope_name, is_open, difficulty_level}
end

function auto_increment_lifts_slopes(lift_id, slope_id)
	return box.space.lifts_slopes:auto_increment{lift_id, slope_id}
end

function auto_increment_messages(sender_id, checked_by_id, text)
	return box.space.messages:auto_increment{sender_id, checked_by_id, text}
end


function count_card_readings(lift_id, date_from, date_query)
	connected_turnstiles = turnstiles.index.index_lift_id:select({lift_id})

	counter = 0

	for k,v in pairs(connected_turnstiles) do
		cur_turnstile_id = v["turnstile_id"]

		card_readings_on_turnstile = card_readings.index.index_turnstile:select({cur_turnstile_id})

		for k,v in pairs(card_readings_on_turnstile) do
			if (v["reading_time"] >= date_from and v["reading_time"] < date_query) then
				counter = counter + 1
			end
		end
	end

	return counter
end


function update_queue_time(lift_id, date_from, date_query)
	card_readings_amount = count_card_readings(lift_id, date_from, date_query)
	lift = lifts:get{lift_id}
	time_delta = date_query - date_from
	new_queue_time = math_module.max(
		math_module.ceil(
			lift["queue_time"] - 
			time_delta  + 
			(card_readings_amount * lift["lifting_time"] * 2 / lift["seats_amount"])
			), 
		0)
	
	lifts:update(lift_id, {{'=', 6, new_queue_time}})
	return new_queue_time
end


-- function get_lifts_by_slope_id(slope_id)
	-- chosen_lifts_slopes = lifts_slopes.index.index_slope_id:select({slope_id})
	-- for k,v in pairs(chosen_lifts_slopes) do
		-- cur_lift_id = v["lift_id"]
	-- end
-- end

-- function get_slopes_by_lift_id(lift_id)
	-- return lifts_slopes.index.index_lift_id:select({lift})
-- end

box.cfg {
   background = false,
   listen = 3301,
}

init()
load__data()


--box.space.users:insert{7777, 0, "tmp_email10", "tmp_password10", 0}
--box.space.users:insert{7771, 0, "admin_email20", "admin_password20", 3}



-----------------------------------------------------------------------------------------test
function dump(o)
   if type(o) == 'table' then
      local s = '{ '
      for k,v in pairs(o) do
         if type(k) ~= 'number' then k = '"'..k..'"' end
         s = s .. '['..k..'] = ' .. dump(v) .. ','
      end
      return s .. '} '
   else
      return tostring(o)
   end
end

--print(dump(get_lifts_by_slope_id(1)))
--print(dump(get_lifts_by_slope_id(2)))


-- auto_increment_card_readings(turnstile_id, card_id, reading_time)
-- auto_increment_lifts(lift_name, is_open, seats_amount, lifting_time, queue_time)
-- auto_increment_turnstiles(lift_id, is_open)



-- auto_increment_lifts('a', true, 10, 100, 0) -- 1
-- auto_increment_lifts('b', true, 20, 500, 100) -- 2


-- auto_increment_turnstiles(1, true) -- 1 (1)
-- auto_increment_turnstiles(1, true) -- 2 (1)

-- auto_increment_turnstiles(2, true) -- 3 (2)


-- 1
-- auto_increment_card_readings(1, 0, 10)
-- auto_increment_card_readings(1, 0, 20)
-- auto_increment_card_readings(2, 0, 10)
-- auto_increment_card_readings(2, 0, 20)
-- auto_increment_card_readings(2, 0, 30)

-- 2
-- auto_increment_card_readings(3, 0, 10)
-- auto_increment_card_readings(3, 0, 20)
-- auto_increment_card_readings(3, 0, 30)


-- print('!!count 1', count_card_readings(1, 0, 40)) -- 5
-- print('!!count 1', count_card_readings(1, 20, 30)) -- 2
-- print('!!count 2', count_card_readings(2, 0, 40)) -- 3
-- print('!!count 2', count_card_readings(2, 20, 31)) -- 2
-- print()

-- print('1')
-- print(update_queue_time(1, 0, 40)) -- 60
-- print(dump(box.space.lifts:select{1}))

-- print(update_queue_time(1, 0, 240)) -- 0
-- print(dump(box.space.lifts:select{1}))






------------------------------------------------------------------------------------------roles

-- box.schema.role.create('unauthorized_user')
-- box.schema.role.grant('unauthorized_user', 'read', 'space', 'lifts', {if_not_exists=true})
-- box.schema.role.grant('unauthorized_user', 'read', 'space', 'slopes', {if_not_exists=true})
-- box.schema.role.grant('unauthorized_user', 'read', 'space', 'lifts_slopes', {if_not_exists=true})


-- box.schema.role.create('authorized_user')
-- box.schema.role.grant('authorized_user', 'unauthorized_user')
-- box.schema.role.grant('authorized_user', 'read,write', 'space', 'messages', {if_not_exists=true})


-- box.schema.role.create('ski_patrol')
-- box.schema.role.grant('ski_patrol', 'authorized_user')
-- box.schema.role.grant('unauthorized_user', 'write', 'space', 'lifts', {if_not_exists=true})
-- box.schema.role.grant('unauthorized_user', 'write', 'space', 'slopes', {if_not_exists=true})
-- box.schema.role.grant('unauthorized_user', 'write', 'space', 'lifts_slopes', {if_not_exists=true})

-- box.schema.role.create('ski_admin')
-- box.schema.role.grant('ski_admin', 'read,write,execute,create,alter,drop', 'universe')


-------------------------------------------------------------------------------------------------temporary


function temporary_function(old, new)
	if (old == nil) then
		print('insert')
	else 
		if (new == nil) then
			print('delete')
		else
			print('update')
		end
	end
end