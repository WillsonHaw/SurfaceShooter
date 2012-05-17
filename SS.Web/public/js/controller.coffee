

id = $("#clientId").val()
name = $("#clientName").val()

PUBNUB.subscribe({
    channel: "client_channel_" + id
    restore: false
    callback: (message) ->
        switch message.message
            when 'update'
                $('.health-value').css('width', message.hp + "%");
                $('.health-text').text(message.hp + "%");
                break;
    connect: () ->
        PUBNUB.publish({
            channel: "surface_channel"
            message: {
                id: id
                charName: name
                message: 'connected'
            }
        })
    disconnect: () ->
        PUBNUB.publish({
            channel: "surface_channel"
            message: {
                id: id
                message: 'disconnected'
            }
        })
    })

publish = (msg, vect) ->
    PUBNUB.publish({
        channel: "surface_channel"
        message: {
            id: id
            message: msg
            vector: vect
        }
    })

leftJoystickDown = (evt) -> publish("move", "down")
leftJoystickMove = (evt) -> publish("move", evt)
leftJoystickUp = (evt) -> publish("move", "up")
rightJoystickDown = (evt) -> publish("fire", "down")
rightJoystickMove = (evt) -> publish("fire", evt)
rightJoystickUp = (evt) -> publish("fire", "up")

leftJoystick = new VirtualJoystick
    container: $("#left-analog")[0]
    mouseSupport: true
    range: 20
    inputDown: leftJoystickDown
    inputMove: leftJoystickMove
    inputUp: leftJoystickUp

rightJoystick = new VirtualJoystick
    container: $("#right-analog")[0]
    mouseSupport: true
    range: 20
    inputDown: rightJoystickDown
    inputMove: rightJoystickMove
    inputUp: rightJoystickUp
