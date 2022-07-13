var exec = require('child_process').execSync;

function excmd(cmd, dsp, msg){
	var dat = exec(cmd);
  console.log('');
  console.log(msg);
  if(dsp){
    console.log('UNIX OUT: ' + dat);
  }
  console.log('Action Complete')
}

excmd('git add .', false, '1)');
excmd('git commit -m "automatic update"', true, '2)');
excmd('git push heroku master', false, '3)');
excmd('heroku open', false, '4)');
