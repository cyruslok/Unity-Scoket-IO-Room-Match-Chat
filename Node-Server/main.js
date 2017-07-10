var io = require('socket.io')({
	transports: ['websocket'],
});
io.attach(4567);

var usernames = {};

var rooms = ['Lobby'];

io.sockets.on('connection', function(socket) {

    socket.on('Test', function(msg){
        console.log('Get Message ' + msg);
    });

    socket.on('userInit', function(username) {
        socket.username = username; socket.room = 'Lobby';
        usernames[socket.id] = username; 
        socket.join('Lobby');

        io.sockets["in"]('Lobby').emit('updatechat', updatechatMsg('System', 'System', username + ' have connected to Lobby'));
        console.log(socket.username + " >>> In >>> " + socket.room);
        //io.sockets["in"](socket.room).emit('updatechat', updatechatMsg(socket.id, username, username + ' has connected to this room'));
        io.sockets.emit('updaterooms', {'id': socket.id ,'rooms':rooms, 'current_room':socket.room, 'room_information': io.sockets.adapter.rooms[socket.room] });
        //socket.emit('updaterooms', rooms, 'Lobby');
    });

    socket.on('create', function(room) {
        rooms.push(room);
        /*socket.leave(socket.room);
        socket.room = room;
        socket.join(room);
        console.log(socket.username + " >>> In >>> " + socket.room);
        io.sockets["in"](socket.room).emit('updatechat', updatechatMsg(socket.id, usernames[socket.id], socket.username + ' has connected to this room'));*/
        io.sockets.emit('updaterooms', {'id': socket.id ,'rooms':rooms, 'current_room':socket.room, 'room_information': io.sockets.adapter.rooms[socket.room] });
    });

    socket.on('sendchat', function(msg) {
        console.log(usernames[socket.id] + " ("+socket.room+"): "+msg);
        io.sockets["in"](socket.room).emit('updatechat', updatechatMsg(socket.id, usernames[socket.id], msg));
    });

    socket.on('switchRoom', function(newroom) {
        socket.leave(socket.room);
        socket.join(newroom);
        io.sockets["in"](socket.room).emit('updatechat', updatechatMsg('System', 'System', socket.username + ' has connected to '+socket.room));
        io.sockets["in"](socket.room).emit('updatechat', updatechatMsg('System', 'System', socket.username + ' has left '+socket.room));
        socket.room = newroom;
        console.log(socket.username + " >>> In >>> " + socket.room);
        io.sockets["in"](socket.room).emit('updatechat', updatechatMsg('System', 'System', socket.username + ' has connected to '+socket.room));
        io.sockets.emit('switchRooms', {'id': socket.id ,'rooms':rooms, 'current_room':socket.room, 'room_information': io.sockets.adapter.rooms[socket.room] });
    });

    socket.on('checkRoomState', function(){
        io.sockets.emit('UpdateRoomState', {'id': socket.id , 'current_room':socket.room, 'room_information': io.sockets.adapter.rooms[socket.room] });
    });

    socket.on('rename', function(name){
        usernames[socket.id] = name;
        socket.username = name;
        console.log(socket.id + "change name >>>>> " + name);
    });

    socket.on('disconnect', function() {
        delete usernames[socket.id];
        io.sockets.emit('updateusers', usernames);
        socket.broadcast.emit('updatechat', 'SERVER', socket.username + ' has disconnected');
        socket.leave(socket.room);
    });
 });


function updatechatMsg(id, username,  msg){
    return {'id':id,'username': username ,'msg':msg};
}
