require('./constants.js');

var SURFACE_CHANNEL = 'surface_channel';
var CLIENT_CHANNEL = 'client_channel';
var PORT = 8899;
var SESSION_SECRET = 'Some Secretness';

var express = require('express');
var pubnub = require('pubnub').init({});

/* Server */
var clients = {};
var lastId = 0;
var server = express.createServer();
server.use(express.cookieParser());
server.use(express.bodyParser());
server.use(express.session({ secret: SESSION_SECRET }));
server.use(express.static(__dirname + '/public'));

server.post('/', function (req, res) {
    var client = {
        charName: req.param('CharName'),
        id: ++lastId
    };

    clients[client.id] = client;
    res.send(client);
});

server.get('/user/:id', function (req, res) {
    var client = clients[req.params.id];
    res.render('controller.jade', { layout: false, user: client });
});

server.get('/', function (req, res) {
    res.render('register.jade', { layout: false });
});

server.listen(PORT);

/* Surface */

/* Web Client */
pubnub.subscribe({
    channel: CLIENT_CHANNEL,
    callback: function(message) {
        console.log(message.texts);
    }
});
