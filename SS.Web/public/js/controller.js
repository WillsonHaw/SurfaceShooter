(function() {
  var id, leftJoystick, leftJoystickDown, leftJoystickMove, leftJoystickUp, name, publish, rightJoystick, rightJoystickDown, rightJoystickMove, rightJoystickUp;
  id = $("#clientId").val();
  name = $("#clientName").val();
  PUBNUB.subscribe({
    channel: "client_channel_" + id,
    restore: false,
    callback: function(message) {
      switch (message.message) {
        case 'update':
          $('.health-value').css('width', message.hp + "%");
          $('.health-text').text(message.hp + "%");
          break;
      }
    },
    connect: function() {
      return PUBNUB.publish({
        channel: "surface_channel",
        message: {
          id: id,
          charName: name,
          message: 'connected'
        }
      });
    },
    disconnect: function() {
      return PUBNUB.publish({
        channel: "surface_channel",
        message: {
          id: id,
          message: 'disconnected'
        }
      });
    }
  });
  publish = function(msg, vect) {
    return PUBNUB.publish({
      channel: "surface_channel",
      message: {
        id: id,
        message: msg,
        vector: vect
      }
    });
  };
  leftJoystickDown = function(evt) {
    return publish("move", "down");
  };
  leftJoystickMove = function(evt) {
    return publish("move", evt);
  };
  leftJoystickUp = function(evt) {
    return publish("move", "up");
  };
  rightJoystickDown = function(evt) {
    return publish("fire", "down");
  };
  rightJoystickMove = function(evt) {
    return publish("fire", evt);
  };
  rightJoystickUp = function(evt) {
    return publish("fire", "up");
  };
  leftJoystick = new VirtualJoystick({
    container: $("#left-analog")[0],
    mouseSupport: true,
    range: 20,
    inputDown: leftJoystickDown,
    inputMove: leftJoystickMove,
    inputUp: leftJoystickUp
  });
  rightJoystick = new VirtualJoystick({
    container: $("#right-analog")[0],
    mouseSupport: true,
    range: 20,
    inputDown: rightJoystickDown,
    inputMove: rightJoystickMove,
    inputUp: rightJoystickUp
  });
}).call(this);
