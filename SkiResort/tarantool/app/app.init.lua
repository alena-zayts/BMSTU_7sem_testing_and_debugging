#!/usr/bin/env tarantool

io_module = require("io")
io_module.stdout:setvbuf("no")
math_module = require("math")

print('start!')



---------------------------------------------------------------------------------------------init tables

local function first_init()
	box.schema.user.create('ski_admin', {if_not_exists = true}, {password = 'Tty454r293300'})
	box.schema.user.passwd('ski_admin', 'Tty454r293300')
	box.schema.user.grant('ski_admin', 'read,write,execute,create,alter,drop', 'universe')
end

local function not_first_init()
	box.space.turnstiles:drop()
	box.space.lifts:drop()
	box.space.slopes:drop()
	box.space.lifts_slopes:drop()
	box.space.users:drop()
end

local function init()
	print('in init!')
	
	box.schema.upgrade()


	pcall(first_init)
	pcall(not_first_init)


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


local function load__data()
	print('in load__data!')
	load_users_data()
	load_turnstiles_data()
	load_lifts_data()
	load_slopes_data()
	load_lifts_slopes_data()
end

----------------------------------------------------------------------------------------------------functions

function auto_increment_users(card_id, user_email, password, permissions)
	return box.space.users:auto_increment{card_id, user_email, password, permissions}
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


box.cfg {
   background = false,
   listen = 3301,
}

init()
--load__data()
print('end of app.init')

