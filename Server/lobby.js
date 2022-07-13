//some redirects to make things easier
process.stdin.on('data', (chunk) =>{var arr = chunk.toString().split(';');arr.pop();for(var data in arr){EventTriggered(arr[data].toString());}});
function cmd(cmd){process.stdout.write('!~' + cmd + ';');}
function system(cmd){process.stdout.write('!~~' + cmd + ';');}
console.log = function (cmd){process.stdout.write(cmd + ';');}

//the good stuff
var users = {};
var order = [];
var lobbyOpen = true;
var readytimeout;
var starting;
var turn;
var classes = {mage:0, cleric:0, rogue:0, warrior:0, random:0};
var enemies = {};
var turnNumber;
var encounterNumber;
var statHolder = {users:{}, enemies:{}};
var Ealive = true;
var Palive = true;
var comboColor = 'l';
var comboNumber = 0;
var comboPool = 0;
var cards;
require('fs').readFile('public/resources/cards.json', (err, res) =>{
    if(err) throw err; 
    else{ 
        cards = JSON.parse(res); 
        for(var i in cards) cards[i].name = i;
    }
});
var encounters;
var encountersRemaining;
require('fs').readFile('public/resources/encounters.json', (err, res) =>{
    if(err) throw err; 
    else {
        encounters = JSON.parse(res); 
        encountersRemaining = JSON.parse(res);
    }
});
var system = {
    users: users,
    enemies: enemies,
    cards: cards,
    encounters: encounters,
    spawn: spawn
};
function EventTriggered(evt){
    //parts of an incomming request are seperated with the : charecter, except for system messages
    //ex: 'Jordan:msg:hello there'
    var arr = evt.split(':');
    if(arr[0] == 'system'){
        arr = arr[1].split(' ');
        if(arr[2] == 'joined'){
            cmd(`msg:[${arr[1]} joined]`);
            users[arr[1]] = {ready:false, class:'random'};
            classes.random++;
            order.push(arr[1]);
            clearTimeout(readytimeout);
            for(var i = 0; i < order.length - 1; i++){
                cmd(`to:${arr[1]}:adduser:` + order[i]);
                if(users[order[i]].class !== undefined) cmd(`to:${arr[1]}:changeuser:${order[i]}:class:${users[order[i]].class}`);
            }
            cmd(`adduser:` + arr[1]);
        }
        else if (arr[2] == 'left'){ 
            classes[users[arr[1]].class]--;
            cmd(`msg:[${arr[1]} left]`);
            delete users[arr[1]];
            order.splice(order.indexOf(arr[1]), 1);
            cmd('removeuser:' + arr[1]);
        }
    }
    else if(arr[1] == 'msg'){
        if(evt.substring(arr[0].length + 5).length > 0){
            var msg = evt.substring(arr[0].length + 5, 205 + arr[0].length);
            cmd('msg:' + decodeURIComponent(arr[0]) + ': ' + msg);
        }
    }
    else if(lobbyOpen){
        switch(arr[1]){
            case 'setclass':
            if (lobbyOpen && users[arr[0]].class !== arr[2] && (classes[arr[2]] < 2  || arr[2] == 'random')) {
                if(classes[users[arr[0]].class] == 2 && users[arr[0]].class !== 'random') cmd(`enable:${users[arr[0]].class}`);
                classes[users[arr[0]].class]--;
                users[arr[0]].class = arr[2];
                classes[arr[2]]++;
                if(classes[arr[2]] == 2 && arr[2] !== 'random') cmd(`disable:${arr[2]}`);
                cmd(`changeuser:${arr[0]}:class:${arr[2]}`);
                //cmd(`msg:[${arr[0]} selected ${arr[2]}]`);
            }
            break;
            case 'toggleready':
            if(lobbyOpen){
                users[arr[0]].ready = !users[arr[0]].ready;
                cmd(`changeuser:${arr[0]}:ready:${users[arr[0]].ready.toString()}`);
                checkStart();
            }
            break;
        }
    }
    else if(users[arr[0]] !== undefined && Palive){
        switch(arr[1]){
            case 'selectcard':
            if(arr[0] == order[turn] && cards[decodeURIComponent(arr[2])] !== undefined){
                var inrange = true;
                var arr2 = arr[3].split(',');
                arr[2] = decodeURIComponent(arr[2]);
                for(var i in arr2){
                    try{
                        if(cards[arr[2]].targeting[i] == 'enemy') var test = enemies[arr2[i]].self;
                        else if(cards[arr[2]].targeting[i] == 'player')  var test = users[arr2[i]].self;
                        else if(cards[arr[2]].targeting[i] == 'ally' && users[arr2[i]].name !== order[turn])  var test = users[arr2[i]].self;
                        else if(cards[arr[2]].targeting[i] == 'any'){
                            try{
                                var test = enemies[arr2[i]].self;
                            }
                            catch(err){
                                var test = users[arr2[i]].self;
                            }
                        }
                    }
                    catch(err){
                        inrange = false;
                        break;
                    }
                }
                var contained = false;
                for(var i in users[arr[0]].self.hand){
                    if(users[arr[0]].self.hand[i].name == arr[2]){
                        contained = true;
                        break;
                    }
                }
                if(inrange && contained){
                    combo(cards[arr[2]]);
                    checkDeath();
                    if(!(Ealive && Palive)) endEncounter();
                    else{
                        var array = {};
                        for(var i = 0; i < 5; i++){
                            if(i < arr2.length){
                                if(cards[arr[2]].targeting[i] == 'enemy') array[i] = enemies[arr2[i]].self;
                                else if(cards[arr[2]].targeting[i] == 'player')  array[i] = users[arr2[i]].self;
                                else if(cards[arr[2]].targeting[i] == 'ally' && users[arr2[i]].name !== order[turn])  array[i] = users[arr2[i]].self;
                                else if(cards[arr[2]].targeting[i] == 'any'){
                                    try{
                                        array[i] = enemies[arr2[i]].self
                                    }
                                    catch(err){
                                        array[i] = users[arr2[i]].self;
                                    }
                                }
                            }
                        }
                        var exec = new Function('system', 'self', 't1', 't2', 't3', 't4', 't5', cards[arr[2]].effect);
                        exec(system, users[arr[0]].self, array[0], array[1], array[2], array[3], array[4]);
                        endTurn();
                    }
                }
            }
            break;
            case 'highlight':
            if(arr[0] == order[turn]){
                if(arr[2] == 'null') cmd('highlight:null');
                var cards2 = users[order[turn]].self.hand;
                for(var i in cards2){
                    if(cards2[i].name == decodeURIComponent(arr[2])){
                        cmd('highlight:' + decodeURIComponent(arr[2]));
                        break;
                    }
                }
            }
            break;
        }
    }
}

class entity {
    constructor(statArr, player, indx) {
        this.health = (statArr.length > 0 ? statArr[0] : 0);
        this.shield = (statArr.length > 1 ? statArr[1] : 0);
        this.range = (statArr.length > 2 ? statArr[2] : 0);
        this.strength = (statArr.length > 3 ? statArr[3] : 0);
        this.intelligence = (statArr.length > 4 ? statArr[4] : 0);
        this.armour = (statArr.length > 5 ? statArr[5] : 0);
        this.wisdom = (statArr.length > 6 ? statArr[6] : 0);
        this.pierce = (statArr.length > 7 ? statArr[7] : 0);
        this.cunning = (statArr.length > 8 ? statArr[8] : 0);
        this.maxhealth = (statArr.length > 0 ? statArr[0] : 0);
        this.player = player;
        this.index = indx;
        this.buffs = [];
        this.locals = {};

        this.indexToStat = function(index){
            switch(index){
                case 0: return this.health;
                case 1: return this.shield;
                case 2: return this.range;
                case 3: return this.strength;
                case 4: return this.intelligence;
                case 5: return this.armour;
                case 6: return this.wisdom;
                case 7: return this.pierce;
                case 8: return this.cunning;
                case 9: return this.maxhealth;
                default: console.error(new Error(index + ' is not a valid stat index'));
            }
        }
        //always go through this
        this.getstat = function(index){
            var debuff = 0;
            var buff = 0;
            for(var i in this.buffs){
                var val = this.buffs[i].stats[index];
                if(this.buffs[i].stats.length > index && this.buffs[i].stats[index] !== 0){
                    var val = this.buffs[i].stats[index];
                    if(val > 0) buff = (buff > val ? buff : val);
                    else debuff = (debuff < val ? debuff : val);
                }
            }
            return this.indexToStat(index) + buff + debuff;
        }
        this.addstat = function(index, value){
            switch(index){
                case 0: return this.health = (this.health + value > this.maxhealth ? this.maxhealth : this.health + value);
                case 1: return this.shield += value;
                case 2: return this.range += value;
                case 3: return this.strength += value;
                case 4: return this.intelligence += value;
                case 5: return this.armour += value;
                case 6: return this.wisdom += value;
                case 7: return this.pierce += value;
                case 8: return this.cunning += value;
                case 9: return this.maxhealth += value;
                default: console.error(new Error(index + ' is not a valid stat index'));
            }
            if(this.player) statHolder.users[this.index][index] += value;
            else statHolder.enemies[this.index][index] += value;
        }
        this.uniqueBuffID = function(id){
            for(var i in this.buffs){
                if(this.buffs[i].id == id) return false;
            }
            return true;
        }
        this.attack = function(targets, damagePHS, damageMAG){
            if(targets.health) targets = [targets];
            //attacking animation outside of loop
            for(var tar in targets){
                var input = [damagePHS, this.getstat(3), targets[tar].getstat(5), this.getstat(7), damageMAG, this.getstat(4), targets[tar].getstat(5), this.getstat(7)];
                for(var i in this.buffs){
                    if(this.buffs[i].effects && this.buffs[i].effects[2] && this.buffs[i].effects[2] !== ''){
                        var exec = new Function('system', 'self', 'input', 'index', 't1', 't2', 't3', 't4', 't5', this.buffs[i].effects[2]);
                        var ret = exec(system, this, input, i, this.buffs[i].locals[0], this.buffs[i].locals[1], this.buffs[i].locals[2], this.buffs[i].locals[3], this.buffs[i].locals[4]);
                        if(ret) input = ret;
                    }
                    if(this.buffs[i].durationtype == 2)this.buffs[i].duration--;
                    if(this.buffs[i].duration == 0) {
                        for(var j in this.buffs[i].stats){
                            if(this.player) statHolder.users[this.index][j] -= this.buffs[i].stats[j];
                            else statHolder.enemies[this.index][j] -= this.buffs[i].stats[j];
                        }
                        this.buffs.splice(i, 1);   
                    }
                }
                var PHSdamage = 0;
                var PHSpenetration = 0;
                var MAGdamage = 0;
                var MAGpenetration = 0
                if(input[0] > 0){
                    PHSdamage = input[0] + input[1] - input[2];
                    PHSdamage = (PHSdamage > 0 ? PHSdamage : 0);
                    PHSpenetration = (input[3] > input[2] ? input[2] : input[3]);
                    PHSpenetration = (PHSpenetration > 0 ? PHSpenetration : 0);
                }
                if(input[4] > 0){
                    MAGdamage = input[4] + input[5] - input[6];
                    MAGdamage = (MAGdamage > 0 ? MAGdamage : 0);
                    MAGpenetration = (input[7] > input[6] ? input[6] : input[7]);
                    MAGpenetration = (MAGpenetration > 0 ? MAGpenetration : 0);
                }

                targets[tar].damage(PHSdamage, MAGdamage, PHSpenetration, MAGpenetration, this.player);
            }
        }
        this.damage = function(damagePHS, damageMAG, penetrationPHS, penetrationMAG, player) {
            if(!damagePHS) damagePHS = 0;
            if(!damageMAG) damageMAG = 0;
            if(!penetrationPHS) penetrationPHS = 0;
            if(!penetrationMAG) penetrationMAG = 0;
            if(!player) player = false;
            //damage animation
            var input = [damagePHS, damageMAG, penetrationPHS, penetrationMAG];
            for(var i in this.buffs){
                if(this.buffs[i].effects && this.buffs[i].effects[3] && this.buffs[i].effects[3] !== ''){
                    var exec = new Function('system', 'self', 'input', 'index', 't1', 't2', 't3', 't4', 't5', this.buffs[i].effects[3]);
                    var ret = exec(system, this, input, i, this.buffs[i].locals[0], this.buffs[i].locals[1], this.buffs[i].locals[2], this.buffs[i].locals[3], this.buffs[i].locals[4]);
                    if(ret) input = ret;
                }
                if(this.buffs[i].durationtype == 3)this.buffs[i].duration--;
                if(this.buffs[i].duration == 0) {
                    for(var j in this.buffs[i].stats){
                        if(this.player) statHolder.users[this.index][j] -= this.buffs[i].stats[j];
                        else statHolder.enemies[this.index][j] -= this.buffs[i].stats[j];
                    }
                    this.buffs.splice(i, 1);   
                }
                
            }
            var damage = (input[0] + input[1] + input[2] + input[3]);
            if(player && comboColor == 'r' && comboNumber > 2) damage += comboPool;
            if(this.shield > damage) this.shield -= damage;
            else{
                this.health -= (damage - this.shield);
                this.shield = 0;
            }
        }
        this.addbuff = function(buffid, arr1, duration, durationtype, arr2, locals){
            if(duration == -1 && buffid == -1){
                for(var i in arr1){
                    this.addstat(parseInt(i), arr1[i]);    
                }
            }
            else if(!this.uniqueBuffID(buffid) && buffid !== -1){
                var index;
                for(var i in this.buffs){
                    if(this.buffs[i].id == buffid) {
                        index = i;
                        break;
                    }
                }
                if(this.buffs[index].duration !== -1) this.buffs[index].duration = (this.buffs[index].duration > duration ? this.buffs[index].duration : duration);
            }
            else if(buffid !== -1 && !arr2) console.error(new Error('buffid is irrelevant')); //BUFF ID IS ONLY FOR EFFECTS
            else{
                var buff = {id:buffid, effects:arr2, duration: duration, durationtype: durationtype, stats: arr1, locals: (locals ? locals : {})}; 
                for(var i in arr1){
                    if(this.player) statHolder.users[this.index][i] += arr1[i];
                    else statHolder.enemies[this.index][i] += arr1[i];
                }
                this.buffs.push(buff);
            }
            //  info:
            //buffid 0xxx = green  2xxx = blue  4xxx = yellow  6xxx = red
            //if duration is -1, it doesn't go away
            //durationtype 0: activates before affectants turn
            //durationtype 1: activates after affectants turn
            //durationtype 2: activates when attacking
            //durationtype 3: activates when taking damage
            //durationtype 4: activates when opposite team is done taking turns
            //durationtype 5: activates when any player takes their turn
            //targeting: ally, enemy, player, any
            //for any effect, use self.locals for stored vars
        }
        this.turnstart = function(){
            for(var i in this.buffs){
                if(this.buffs[i].effects && this.buffs[i].effects[0] && this.buffs[i].effects[0] !== ''){
                    var exec = new Function('system', 'self', 'index', 't1', 't2', 't3', 't4', 't5', this.buffs[i].effects[0]);
                    exec(system, this, i, this.buffs[i].locals[0], this.buffs[i].locals[1], this.buffs[i].locals[2], this.buffs[i].locals[3], this.buffs[i].locals[4]);
                }
                if(this.buffs[i].durationtype == 0)this.buffs[i].duration--;
                if(this.buffs[i].duration == 0) {
                    for(var j in this.buffs[i].stats){
                        if(this.player) statHolder.users[this.index][j] -= this.buffs[i].stats[j];
                        else statHolder.enemies[this.index][j] -= this.buffs[i].stats[j];
                    }
                    this.buffs.splice(i, 1);   
                }
            }
        }
        this.turnend = function(){
            for(var i in this.buffs){
                if(this.buffs[i].effects && this.buffs[i].effects[1] && this.buffs[i].effects[1] !== ''){
                    var exec = new Function('system', 'self', 'index', 't1', 't2', 't3', 't4', 't5', this.buffs[i].effects[1]);
                    exec(system, this, i, this.buffs[i].locals[0], this.buffs[i].locals[1], this.buffs[i].locals[2], this.buffs[i].locals[3], this.buffs[i].locals[4]);
                }
                if(this.buffs[i].durationtype == 1)this.buffs[i].duration--;
                if(this.buffs[i].duration == 0) {
                    for(var j in this.buffs[i].stats){
                        if(this.player) statHolder.users[this.index][j] -= this.buffs[i].stats[j];
                        else statHolder.enemies[this.index][j] -= this.buffs[i].stats[j];
                    }
                    this.buffs.splice(i, 1);   
                }
            }
        }
        this.teamstart = function(){
            this.shield = 0;
            for(var i in this.buffs){
                if(this.buffs[i].effects && this.buffs[i].effects[4] && this.buffs[i].effects[4] !== ''){
                    var exec = new Function('system', 'self', 'index', 't1', 't2', 't3', 't4', 't5', this.buffs[i].effects[4]);
                    exec(system, this, i, this.buffs[i].locals[0], this.buffs[i].locals[1], this.buffs[i].locals[2], this.buffs[i].locals[3], this.buffs[i].locals[4]);
                }
                if(this.buffs[i].durationtype == 4)this.buffs[i].duration--;
                if(this.buffs[i].duration == 0) {
                    for(var j in this.buffs[i].stats){
                        if(this.player) statHolder.users[this.index][j] -= this.buffs[i].stats[j];
                        else statHolder.enemies[this.index][j] -= this.buffs[i].stats[j];
                    }
                    this.buffs.splice(i, 1);   
                }
            }
        }
        this.onturn = function(){
            for(var i in this.buffs){
                if(this.buffs[i].effects && this.buffs[i].effects[5] && this.buffs[i].effects[5] !== ''){
                    var exec = new Function('system', 'self', 'index', 't1', 't2', 't3', 't4', 't5', this.buffs[i].effects[5]);
                    exec(system, this, i, this.buffs[i].locals[0], this.buffs[i].locals[1], this.buffs[i].locals[2], this.buffs[i].locals[3], this.buffs[i].locals[4]);
                }
                if(this.buffs[i].durationtype == 5)this.buffs[i].duration--;
                if(this.buffs[i].duration == 0) {
                    for(var j in this.buffs[i].stats){
                        if(this.player) statHolder.users[this.index][j] -= this.buffs[i].stats[j];
                        else statHolder.enemies[this.index][j] -= this.buffs[i].stats[j];
                    }
                    this.buffs.splice(i, 1);   
                }
            }
        }
        if(player){
            this.hand = [];
            this.deck = [];
            this.discard = [];
            this.draw = function(times){
                for(var xjsbv = 0; xjsbv < times; xjsbv++){
                    var index = Math.floor(Math.random() * deck.length);
                    this.hand.push(deck[index]);
                    deck.splice(index, 1);
                    if(deck.length == 0){
                        this.hand = this.hand.concat(this.discard);
                        this.discard = [];
                    }
                }
            }
            this.discard = function(){
                this.discard = this.discard.concat(this.hand);
                this.hand = [];
            }
        }
    }
}

function shuffleArray(array) {
    for (var i = array.length - 1; i > 0; i--) {
        var j = Math.floor(Math.random() * (i + 1));
        var temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
    return array;
}

function checkStart(){
    var prev = starting;
    starting = true;
    for(var i in users){if(!users[i].ready) {starting = false; break;}}
    if(starting) readytimeout = setTimeout(function(){startGame()}, 5500);
    else clearTimeout(readytimeout);
    if(starting !== prev) cmd('readytimertoggle:' + starting);
}

function startGame(){
    system('unlist');
    cmd('initialize');
    lobbyOpen = false;
    var clArr = [];
    for(var i in classes) {
        if(i !== 'random'){
            if(classes[i] == 0){
                clArr.push(i);
                clArr.push(i);
            }
            else if(classes[i] == 1) clArr.push(i);
        }
    }
    clArr = shuffleArray(clArr);
    for(var i in users){
        if(users[i].class == 'random'){
            users[i].class = clArr.pop();
            cmd(`changeuser:${i}:class:${users[i].class}`);
            cmd(`msg:[${i} was assigned ${users[i].class}]`);
        }
    }
    for(var i in order) users[order[i]].order = i;

    for(var i in users){
        var arr;
        var deck = [ cards.Slice, cards['Double Strike'], cards.Block, cards.Heal, cards['Magic Blast'], cards.Explosion, cards.Planning, cards.Mark];
        switch(users[i].class){
            case 'warrior':
            arr = [100, 0, 7];
            deck.push(cards.Bash); 
            deck.push(cards['Iron Blow']); 
            break;
            case 'cleric':
            arr = [90, 0, 8];
            deck.push(cards.Blessing); 
            deck.push(cards.Courage); 
            break;
            case 'mage':
            arr = [80, 0, 9];
            deck.push(cards.Curse); 
            deck.push(cards.Fireball); 
            break;
            case 'rogue':
            arr = [70, 0, 10];
            deck.push(cards.Preperation); 
            deck.push(cards['Sneak Attack']); 
            break;
        }
        users[i].self = new entity(arr, true, i);
        users[i].self.deck = deck;
    }
    encounterNumber = 1;
    startEncounter();
}

function startEncounter(){
    turnNumber = 0;
    comboPool = 0;
    comboNumber = 0;
    comboColor = 'w';
    cmd(`combodisplay:${comboColor}:${comboNumber}:${comboPool}`);
    enemygen();
    statHolder = {users:{}, enemies:{}};
    for(var i in users) statHolder.users[i] = [0,0,0,0,0,0,0,0,0,0];
    for(var i in enemies) statHolder.enemies[i] = [0,0,0,0,0,0,0,0,0,0];
    startRound();
}

function startRound(){
    for(var i in users) users[i].self.shield = 0;
    turnNumber++;
    updateStats(true);
    cmd('msg:[round ' + turnNumber + ' has begun]');
    for(var i in users) users[i].self.teamstart();
    turn = 0;
    startTurn();
}

function startTurn(){
    users[order[turn]].self.turnstart();
    updateStats(true);
    cmd('setturn:' + order[turn]);
    users[order[turn]].self.draw(3);
    var obj = {};
    for(var i in users[order[turn]].self.hand){
        obj[i] = {};
        obj[i].name = users[order[turn]].self.hand[i].name;
        obj[i].description = users[order[turn]].self.hand[i].description;
        obj[i].targeting = users[order[turn]].self.hand[i].targeting;
    }
    cmd('carddisplay:' + JSON.stringify(obj));
}

function endTurn(){
    updateStats(true);
    users[order[turn]].self.discard();
    users[order[turn]].self.turnend();
    for(var i in users) users[i].self.onturn();
    for(var i in enemies) enemies[i].self.onturn();
    updateStats(true);
    checkDeath();
    if(!(Ealive && Palive)) endEncounter();
    else{
        turn++;
        if(turn < order.length){
            startTurn();
        }
        else endRound();
    }
}

function endRound(){
    for(var i in enemies) enemies[i].self.teamstart();
    for(var i in enemies){
        enemies[i].self.turnstart();
        //animations
        var exec = new Function('system', 'self', 't1', 't2', 't3', 't4', 't5', enemies[i].effect);
        var array = calculateTargets(5);
        exec(system, enemies[i].self, array[0], array[1], array[2], array[3], array[4]);
        enemies[i].self.turnend();
        updateStats(true);
        checkDeath()
        if(!(Ealive && Palive)) break;
    }
    if(!(Ealive && Palive)) endEncounter();
    else startRound();
}

function endEncounter(){
    if(Ealive){
        cmd('setturn:system');
        cmd('loss');
    }
    else {
        encounterNumber++;
        console.log('you did it!');
        //select from skilltree
    }
}

function combo(card){
    if(card.color == comboColor) {
        comboNumber++;
        if(comboNumber == 3){
            switch(comboColor){
                case 'r':
                comboPool = 10;
                break;
                case 'g':
                comboPool = 10;
                break;
                case 'y':
                comboPool = 5;
                for(var i in users) users[i].self.addbuff(-1, [comboPool], -1);
                break;
                case 'b':
                comboPool = 10;
                for(var i in enemies) enemies[i].self.damage(comboPool);
                break;
            }
        }
        else if(comboNumber > 3){
            switch(comboColor){
                case 'r':
                comboPool += 2;
                break;
                case 'g':
                comboPool += 10 + (2 * (comboNumber - 3));
                break;
                case 'y':
                comboPool += 1;
                for(var i in users) users[i].self.addbuff(-1, [comboPool], -1);
                break;
                case 'b':
                comboPool += 2;
                for(var i in enemies) enemies[i].self.damage(comboPool);
                break;
            }
        }
    }
    else {
        if(comboColor == 'g' && comboNumber > 2){
            var index;
            var hp;
            for(var i in enemies){
                var h = enemies[i].self.getstat(0);
                if(hp == undefined || hp < h){
                    index = i;
                    hp = h;
                }
            }
            if(enemies[index].self.shield > comboPool) enemies[index].self.shield -= comboPool;
            else{
                enemies[index].self.health -= (comboPool - enemies[index].self.shield);
                enemies[index].self.shield = 0;
            }
        }
        comboColor = card.color;
        comboNumber = 1;
        comboPool = 0;
    }
    cmd(`combodisplay:${comboColor}:${comboNumber}:${comboPool}`);
}

function updateStats(overide){
    //if(overide){
        var obj = {users:{}, enemies:{}};
        for(var i in users){
            obj.users[i] = {};
            obj.users[i][0] = users[i].self.getstat(0);
            obj.users[i][1] = users[i].self.getstat(1);
            obj.users[i][2] = users[i].self.getstat(2);
            obj.users[i][3] = users[i].self.getstat(3);
            obj.users[i][4] = users[i].self.getstat(4);
            obj.users[i][5] = users[i].self.getstat(5);
            obj.users[i][6] = users[i].self.getstat(6);
            obj.users[i][7] = users[i].self.getstat(7);
            obj.users[i][8] = users[i].self.getstat(8);
            obj.users[i][9] = users[i].self.maxhealth;
        }
        for(var i in enemies){
            obj.enemies[i] = {};
            obj.enemies[i][0] = enemies[i].self.getstat(0);
            obj.enemies[i][1] = enemies[i].self.getstat(1);
            obj.enemies[i][2] = enemies[i].self.getstat(2);
            obj.enemies[i][3] = enemies[i].self.getstat(3);
            obj.enemies[i][4] = enemies[i].self.getstat(4);
            obj.enemies[i][5] = enemies[i].self.getstat(5);
            obj.enemies[i][6] = enemies[i].self.getstat(6);
            obj.enemies[i][7] = enemies[i].self.getstat(7);
            obj.enemies[i][8] = enemies[i].self.getstat(8);
            obj.enemies[i][9] = enemies[i].self.maxhealth;
        }
        cmd('statupdate:true:' + JSON.stringify(obj));
        /*statHolder = {users:{}, enemies:{}};
        for(var i in users) statHolder.users[i] = [0,0,0,0,0,0,0,0,0,0];
        for(var i in enemies) statHolder.enemies[i] = [0,0,0,0,0,0,0,0,0,0];
    }
    else{
        cmd('statupdate:false:' + JSON.stringify(statHolder));
        statHolder = {users:{}, enemies:{}};
        for(var i in users) statHolder.users[i] = [0,0,0,0,0,0,0,0,0,0];
        for(var i in enemies) statHolder.enemies[i] = [0,0,0,0,0,0,0,0,0,0];
    }*/
}

function lcmIter(arr,length){
    if(length == 2) return lcm(arr[0],arr[1]);
    else return lcm(arr[length-1],lcmIter(arr,length-1));
} function gcd(a, b) {
    return !b ? a : gcd(b, a % b);
} function lcm(a, b) {
    return (a * b) / gcd(a, b);   
} 

function calculateTargets(val){
    var ranges = [];
    var references = [];
    for(var i in users){
        ranges.push(users[i].self.getstat(2));
        references.push(i);
    }
    var lcm = (ranges.length == 1 ? ranges[0] : lcmIter(ranges, ranges.length));
    var chances = [];
    var total = 0;
    for(var i in ranges){
        chances.push(lcm / ranges[i]);
        total += (lcm / ranges[i]);
    }
    // (1 / range) * (lcm / total)
    var targets = [];
    for(var xjsbv = 0; xjsbv < val; xjsbv++){
        var num = Math.floor(Math.random() * total);
        var i = 0;
        while(true){
            if(chances[i] < num){
                num -= chances[i];
                i++;
            }
            else{
                targets.push(users[references[i]].self);
                break;
            }
        }
    }
    return targets;
}

function checkDeath(){
    for(var i in enemies){
        if(enemies[i].self.getstat(0) <= 0){
            cmd('removeenemy:' + i);
            delete enemies[i];
        }
    }
    if(Object.keys(enemies).length == 0) Ealive = false;
    for(var i in users){
        if(users[i].self.getstat(0) <= 0){
            cmd('changeuser:' + i + ':class:dead');
            order.splice(order.indexOf(i), 1);
        }
    }
    if(order.length == 0) Palive = false;
}

function enemygen(){
    var difficulty;
    //make sure to revive players before calling this function
    switch(order.length){
        case 1: case 2: difficulty = 'easy'; break;
        case 3: case 4: difficulty = 'medium'; break;
        case 5: case 6: difficulty = 'hard'; break;
        case 7: case 8: difficulty = 'insane'; break;
    }

    if(encounterNumber % 5 == 0){
        //bosses n stuff
    }
    else if(encounterNumber > 30){
        //modifiers n stuff
        //also random enemy gen (however thats gonna work)
    }
    else{
        var floornum = Math.floor(encounterNumber / 10) + 1;
        var indx = Math.floor(encountersRemaining['floor' + floornum.toString()].length * Math.random());
        spawn(encountersRemaining['floor' + floornum.toString()][indx][difficulty]);
        encountersRemaining['floor' + floornum.toString()].splice(indx, 1);
    }
}

function spawn(enemies){
    if(enemies.health) enemies = [enemies];
    //input: effect, name, stats
    var obj = {};
    for(var i in enemies){
        var enemy = {};
        enemy.name = enemies[i].name;
        enemy.effect = enemies[i].effect;
        enemy.id = genID();
        enemy.self = new entity(enemies[i].stats, false, enemy.id);
        //add some other effects that activate on x
        enemies[enemy.id] = enemy;

        obj[enemy.id] = {name: enemy.name};
    }
    cmd('enemydisplay:' + JSON.stringify(obj));
}

function genID(){
    var id = Math.floor(Math.random() * 100000);
    while(enemies[id] !== undefined || order.includes(id)){
        id = Math.floor(Math.random() * 100000);
    }
    return id;
}