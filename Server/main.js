const express = require('express');
const stringify = require('json-stringify-safe');
const bodyParser = require('body-parser');
const requestIp = require('request-ip');
const fs = require('fs');
const cp = require('child_process');
const expressWs = require('express-ws');
const { Client } = require('pg');
const crypto = require('crypto');
const path = require ('path');
const port = process.env.PORT || 80;
const connection = process.env.DATABASE_URL || 'postgres://localhost:5432/database_name';


//webhost + childprocesses + websockets
var servers = {}; //non-encoded, BE CAREFUL (encode in future)
var names = {}; //uri component encoded
var IDs = {}; 
var connections = {}; 
var app = express();
var wsInstance = expressWs(app);
app.engine('.html', require('ejs').__express);
app.set('views', __dirname + '/views');
app.set('view engine', 'html');
app.use(express.static(__dirname + '/public'));
app.use(bodyParser.text({type:'text/plain'}));
app.use(function(req, res, next) { 
	req.ip = requestIp.getClientIp(req);
	req.cookies = {};
	if(req.headers.cookie !== undefined){
		var match = req.headers.cookie.match(/userID=[\w]{10,10}/g);
		if(match !== null){ 
			req.cookies.userID = match[0].substring(7);
			res.cookie('userID', req.cookies.userID);
		}
	}
	//uri encoded strings will contain -_.!~*'()% and a-z A-Z 0-9 
	if(req.body !== null) req.body = encodeURIComponent(req.body);
	for(var i in req.query){req.query[i] = encodeURIComponent(req.query[i]);}


					//QUERY AND BODY IS ENCODED


	//fs.writeFile('test.json', stringify(req, null, '\t'), (err)=>{if(err) console.error(err);});
	next();
});

app.get('/', function(req, res){ 
	//*
	if(names[req.cookies.userID] !== undefined){
		res.statusCode = 200; 
		res.render('lobbyRedirect'); 
	}
	else {
		res.statusCode = 200; 
		res.render('main'); 
	}/**\/
	res.cookie('userID', '0000000000');
	IDs['CryoFlame'] = '0000000000';
	names['0000000000'] = 'CryoFlame';
	res.statusCode = 200;
	res.render('lobbyRedirect');/**/
});
app.get('/lobby', function(req, res){
	if(names[req.cookies.userID] == undefined){
		res.statusCode = 200; 
		res.render('mainRedirect'); 
	}
	else if(req.query.l == undefined){
		res.statusCode = 200; 
		res.render('lobbyBrowser'); 
	}
	else if(servers[req.query.l] !== undefined && (servers[req.query.l].password == null || servers[req.query.l].authorized.includes(req.cookies.userID))){
		res.statusCode = 200; 
		res.cookie('username', names[req.cookies.userID]);
		res.render('game'); 
	}
	else{
		res.statusCode = 200; 
		res.render('lobbyRedirect');
	}
});

app.get('/data', (req, res) => {
	switch(req.query.q){
		case 'servers':
		var dat = {};
		for(var i in servers){
			dat[i] = {};
			dat[i].users = servers[i].users;
			dat[i].open = servers[i].open;
			dat[i].name = servers[i].name;
			dat[i].password = servers[i].password !== null;
			dat[i].connected = servers[i].connected;
		}
		res.status(200).send(JSON.stringify(dat));
		break;
		case 'port':
		res.status(200).send(port.toString());
		break;
		case 'getcolor':
		if(names[req.cookies.userID] !== undefined){
			db.query("SELECT themehue FROM users WHERE username = $1", [names[req.cookies.userID]], (err, ret) =>{
				if(ret.rows.length == 1){
					if(ret.rows[0].themehue !== undefined) res.append('themeHue', ret.rows[0].themehue);
					else res.append('themeHue', '210');
					res.status(200).send('success');
				}
				else res.status(500).send('an error has occured');
			});
		}
		break;
		default:
		res.status(400).send('bad request');
	}
});
app.post('/data', (req, res) =>{
	switch(req.query.q){
		case 'joinserver':
		if(servers[req.body] !== undefined && req.body && servers[req.body].connected.length < 8 && (servers[req.body].password == null || servers[req.body].password == req.headers.password)){
			servers[req.body].authorized.push(req.cookies.userID);
			res.status(200).send('success');
		}
		else res.status(400).send('bad request');
		break;
		case 'createserver':
		if(servers[req.body] == undefined && req.body && encodeURIComponent(req.body).match(/[-_\.!~\*'\(\)]/g) == null && decodeURIComponent(req.body).length <= 24){ 
			var app = cp.spawn('node lobby', {shell:true, stdio:'pipe'});
			var server = req.body;
			app.stderr.on('data', (chunk)=>{
				console.error('lobby error:\n' + chunk.toString());
			});
			app.stdout.on('data', (chunk)=>{
				var arr = chunk.toString().split(';');
				if(arr[arr.length - 1] == '') arr.pop();
				for(var i in arr){
					var input = arr[i];
					if(!input) console.log(input + ' ERR ' + chunk + ' ERR ' + arr + ' ERR ' + i);
					if(input.match(/^!~~/) !== null){
						var arr2 = input.substring(3).split(':');
						switch(arr2[0]){
							case 'unlist':
							servers[server].open = false;
							break;
						}
					}
					else if(input.match(/^!~/) !== null){
						input = input.substring(2).split(':');
						if(input[0] == 'to'){
							var id = IDs[input[1]];
							input.splice(0, 2);
							if(servers[server].WS[id] == undefined){
								console.log(id);
								console.log(stringify(servers[server].WS));
							}
							servers[server].WS[id].send(input.join(':')); //err here (investigate)
						}
						else{
							for(var i in servers[server].WS){
								servers[server].WS[i].send(input.join(':'));
							}
						}
					}
					else console.log(input);
				}
			});
			servers[server] = {
				authorized:[req.cookies.userID],
				password:(req.headers.password !== '' ? req.headers.password : null),
				users: [],
				connected: [],
				name: server,
				process: app,
				WS:{},
				open:true,
				needhtml:[]
			};
			res.status(200).send('success');
		}
		else res.status(409).send('failure');
		break;
		case 'setcolor':
		if(names[req.cookies.userID] !== undefined && decodeURIComponent(req.body).match(/^\d{1,3}$/) !== null){
			db.query("SELECT themehue FROM users WHERE username = $1", [names[req.cookies.userID]], (err, ret) =>{
				if(ret.rows.length == 1){
					db.query("UPDATE users SET themeHue = $1 WHERE username = $2", [req.body, names[req.cookies.userID]], (err) =>{
						if(err) res.status(500).send('an error has occured');
						else res.status(200).send('success');
					});
				}
				else res.status(500).send('an error has occured');
			});
		}
		break;
		case 'login':
		if(req.body && decodeURIComponent(req.body).length <= 24 && req.headers.username && req.headers.username.length <= 24){
			req.headers.username = encodeURIComponent(req.headers.username);
			db.query("SELECT password FROM users WHERE username = $1", [req.headers.username], (err, ret) =>{
				if(ret.rows.length == 1 && encodeURIComponent(req.headers.username).match(/[-_.!~*'()%]/) == null){
					var hash = crypto.createHash('sha256');
					hash.update(decodeURIComponent(req.body));
					var pw = hash.digest('hex');
					if(ret.rows[0].password == pw){
						var id = IDgen();
						if(req.cookies.userID !== undefined) delete names[req.cookies.userID];
						IDs[req.headers.username] = id;
						names[id] = req.headers.username;
						res.append('userID', id);
						res.status(200).send('success');
					}
					else res.status(403).send('incorrect password');
				}
				else res.status(400).send('username not found');
			});
		}
		break;
		case 'register':
		if(req.body && decodeURIComponent(req.body).length <= 24 && req.headers.username && req.headers.username.length > 0 && req.headers.username.length <= 24 && req.headers.username !== 'system' && req.headers.username.match(/^\d{1,2}$/) == null && encodeURIComponent(req.headers.username).match(/[-_.!~*'()%]/) == null){
			req.headers.username = encodeURIComponent(req.headers.username);
			db.query("SELECT username FROM users WHERE username = $1", [req.headers.username], (err, ret) =>{
				if(ret.rows.length !== 0)res.status(400).send('that username has already been taken');
				else  if(decodeURIComponent(req.body).length < 6)res.status(400).send('your password must be 6 charecters or longer');
				else{
					var hash = crypto.createHash('sha256');
					hash.update(decodeURIComponent(req.body));
					var pw = hash.digest('hex');
					db.query("INSERT INTO users (username, password) VALUES ($1, $2)", [req.headers.username, pw], (err) =>{
						if(err) {console.error(err);res.status(500).send('there has been an error in creating your account, please try later');}
						else{
							var id = IDgen();
							IDs[req.headers.username] = id;
							names[id] = req.headers.username;
							res.append('userID', id);
							res.status(200).send('success');
						}
					});
				}
			});
		}
		else res.status(400).send("invalid username");
		break;
		case 'logout':
		if(req.cookies.userID){
			for(var i in connections){
				connections[i].close();
			}
			delete IDs[names[req.cookies.userID]];
			delete names[req.cookies.userID];
			res.status(200).send('success');
		}
		else res.status(400).send('failure');
		break;
		default:
		res.status(400).send('bad request');
	}
});

app.ws('/ws', (ws, req) =>{
	try{
		if(servers[req.ws.protocol] !== undefined && !servers[req.ws.protocol].connected.includes(req.cookies.userID)){
			var server = req.ws.protocol;
			var userID = req.cookies.userID;
			var user = names[req.cookies.userID];
			var open = !!servers[server].open;
			servers[server].users.push(userID);
			servers[server].WS[userID] = ws;
			if(open)servers[server].connected.push(userID);
			if(connections[userID] == undefined) connections[userID] = [];
			connections[userID].push(ws);
			ws.on('message', function(msg) {
				var arr = msg.split(':');
				for(var i in arr){
					arr[i] = encodeURIComponent(arr[i]);
				}
				msg = arr.join(':');
				if(msg.toString() == 'dispose');
				else if(msg.split(':')[0] == 'html') {
					for(var i in servers[server].needhtml){
						servers[server].needhtml[i].send('sethtml:' + msg.substring(5));
					}
				}
				else {
					servers[server].process.stdin.write(user + ':' + msg.toString() + ';');
				}
			});
			ws.on('close', function() {
				connections[userID].splice(connections[userID].indexOf(ws), 1);
				servers[server].users.splice(servers[server].users.indexOf(userID), 1);
				delete servers[server].WS[userID];
				if(Object.keys(servers[server].WS).length == 0){
					if(process.platform.match(/win/) !== null) cp.spawn("taskkill", ["/pid", servers[server].process.pid, '/f', '/t']);
					else servers[server].process.kill();
					delete servers[server];
				}
				else if (open){
					servers[server].process.stdin.write('system:user ' + user.toString() + ' left;');
					servers[server].connected.splice(servers[server].connected.indexOf(userID), 1);
				}
			});
			if(open) servers[server].process.stdin.write('system:user ' + user.toString() + ' joined;');
			else{
				ws.send('logdata');
				servers[server].needhtml.push(ws);
				for(var i in servers[server].connected){
					servers[server].WS[servers[server].connected[i]].send('gethtml');
					break;
				}
			}
		}
		else if(servers[req.ws.protocol] == undefined)
			ws.close();
		else
			ws.close();
	} catch(err){console.error(err);}
});

app.listen(port, ()=>{
	console.log('The website is now being hosted');
}); 


//*database
process.env.PGUSER = process.env.PGUSER || 'user_name';
process.env.PGPASSWORD = process.env.PGPASSWORD || 'password';
var db = new Client({
	connectionString: connection,
	ssl: process.platform.match(/win/) == null
});
db.connect();


db.query('CREATE TABLE users()', (err) =>{if(err.code !== '42P07') console.error(err);});
db.query('ALTER TABLE users ADD username VARCHAR(72)', (err) =>{if(err.code !== '42701') console.error(err);});
	//username is stored url-encoded
db.query('ALTER TABLE users ADD password CHAR(64)', (err) =>{if(err.code !== '42701') console.error(err);});
	//password is stored as a hashed urldecoded hex-value
db.query('ALTER TABLE users ADD themeHue SMALLINT', (err) =>{if(err.code !== '42701') console.error(err);});

function IDgen(){
	var chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890"
	var id = '';
	for (let i = 0; i < 10; i++) {
		id += chars.charAt(Math.floor(Math.random() * chars.length))
	}
	 if(names[id] == undefined) return id;
	 else return IDgen();
}/**/