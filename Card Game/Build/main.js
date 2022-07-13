require('http').createServer((req, res) => {
    if(req.method == 'GET')
        res.write(require('fs').readFileSync(req.url == '/'?'index.html': req.url.substring(1)));
    res.end();
}).listen(80, ()=>{console.log('server is up');});