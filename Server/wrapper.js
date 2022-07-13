console.log('running...');
var cp = require('child_process');
var fs = require('fs');
var currentCmd = 'run';
var state = 'start';
var jsonCmd;
var lastcmd;

function delaytime(delay){
	return new Promise(resolve=>{
		setTimeout(()=>{
			resolve('');
		}, delay);
	});
}

function getTimelocal(){
	var date = new Date();
	var hour = date.getHours();
	var mmm = 'am';
	if(hour > 12) {hour -= 12; mmm = 'pm';}
	var min = date.getMinutes();
	if(min < 10) min = '0' + min;
	return hour + ':' + min + ' ' + mmm;
}

function getDatelocal(){
	var date = new Date();
	var month = '';
	switch(date.getMonth()){
		case 1: month = 'January'; break;
		case 2: month = 'February'; break;
		case 3: month = 'March'; break;
		case 4: month = 'April'; break;
		case 5: month = 'May'; break;
		case 6: month = 'June'; break;
		case 7: month = 'July'; break;
		case 8: month = 'August'; break;
		case 9: month = 'September'; break;
		case 10: month = 'October'; break;
		case 11: month = 'November'; break;
		case 12: month = 'December'; break;
	}
	var suffix = 'th';
	switch(date.getDate()){
		case 1: suffix = 'st'; break;
		case 2: suffix = 'nd'; break;
		case 3: suffix = 'rd'; break;
		case 21: suffix = 'st'; break;
		case 22: suffix = 'nd'; break;
		case 23: suffix = 'rd'; break;
		case 31: suffix = 'st'; break;
	}
	return month + ' ' +  date.getDate() + suffix + ', ' + date.getFullYear();
}

function run(){
	if(currentCmd !== 'run'){
		sendLog('now running');
		currentCmd = 'run';
	}

	if(state == 'start'){
		sendLog('shell successfuly started');
	}
	else if(state == 'crash'){
		sendLog('recovered from crash');
	}
	else if(state == 'restart'){
		sendLog('program restarted');
	}
	state = 'crash';
	var app = cp.spawn('node main', {shell:true, stdio:[process.stdin, 'pipe', 'pipe']});
	app.on('close', ()=>{	
		check1();
	});
	app.stderr.on('data', (chunk)=>{
		sendErr(chunk);
		//sendLog(chunk);
	});
	app.stdout.on('data', (chunk)=>{
		if((chunk.toString()).includes('~~restart~~')){
			fs.readFile('main.js', function(err, data) {
	    		if(err) sendErr(err);
	    		else{
			    	try{
			    		var vm = require('vm');
			    		var script = new vm.Script(data);

			    		state = 'restart';
		    			if(process.platform.match(/win/) !== null) cp.spawn("taskkill", ["/pid", servers[server].process.pid, '/f', '/t']);
						else servers[server].process.kill();
			    	}
			    	catch(err){sendErr(err);}
			    }
		    });
		}
		else{
			sendLog(chunk);
		}
	});
}

function check1(){
	fs.readFile('metadata.json', function(err, data) {
		if (err) throw err;
		else {
			var obj = JSON.parse(data);
			check2(obj.command);
		}
	});
}

async function check2(data){
	if (data == 'pause'){
		if(currentCmd !== data){
			sendLog('now paused');
			currentCmd = 'pause';
		}
		await delaytime(5000);
		check1();
	}
	else if(data == 'debug'){
		sendLog('entering debug mode');
		currentCmd = 'debug';
		fs.readFile('metadata.json', function(err, data) {
			if (err) throw err;
			else {
				var obj = JSON.parse(data);
				obj.command = 'pause';
				fs.writeFile('metadata.json', JSON.stringify(obj, null, '\t'), function (err) {if (err) throw(err); else run();});
			}
		});
	}
	else
		run();
}

function newcmd(){
	var date = new Date();
	var h = date.getHours();
	var m = date.getMinutes();
	if(((60 * h) + m) > 90 + lastcmd)
		console.log('  |  ');
	lastcmd = (60 * h) + m;
}

function sendErr(err){
	if(process.platform.match(/win/) == null) console.error(err.toString());
	fs.appendFile('errorlog.txt', getTimelocal() + ' ' + getDatelocal() + ': \n' + err + '\n\n\n\n', function (err) {
		if (err) throw(err);
		else {
			newcmd();
			console.log(getTimelocal() + ': error sent to error log');
		}
	});
}

function sendLog(log){
	newcmd();
	log = log.toString();
	log = log.replace(/\n+$/, '');
	console.log(getTimelocal() + ': ' + log);
}










fs.readFile('metadata.json', function(err, data) {
	if (err) throw err;
	else {
		var obj = JSON.parse(data);
		jsonCmd = obj.command;
		fs.writeFile('metadata.json', JSON.stringify(obj, null, '\t'), function (err) {if (err) throw(err);});
	}
});

var date = new Date();
var h = date.getHours();
var m = date.getMinutes();
lastcmd = (60 * h) + m;

console.log('\n' + getDatelocal() + ': ');
run();