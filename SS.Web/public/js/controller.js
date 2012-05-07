var drawControls = function (id) {
    //Styles
    var padding = 15;
    var btnWidth = 65;
    var btnHeight = 65;
    var btnConfig = {
        width: btnWidth,
        height: btnHeight,
        cornerRadius: 10,
        fill: "#f0f0f0",
        stroke: "#bbb",
        strokeWidth: 1
    };

    //Stage
    var stage = new Kinetic.Stage({ container: "container", width: btnWidth * 5 + padding * 4 + 2, height: btnHeight * 3 + padding * 2 + 2 });

    //Background
    var bgLayer = new Kinetic.Layer();
    var bgRect = new Kinetic.Rect({ width: stage.getWidth(), height: stage.getHeight(), cornerRadius: 10, fill: "#eeeeee", stroke: "#dddddd", strokeWidth: 1 });

    bgLayer.add(bgRect);

    //Components
    var controlLayer = new Kinetic.Layer();
    var upRect = new Kinetic.Rect($.extend(btnConfig, { x: btnWidth + padding, y: padding }));
    var downRect = new Kinetic.Rect($.extend(btnConfig, { x: btnWidth + padding, y: btnHeight * 2 + padding }));
    var leftRect = new Kinetic.Rect($.extend(btnConfig, { x: padding, y: btnHeight + padding }));
    var rightRect = new Kinetic.Rect($.extend(btnConfig, { x: btnWidth * 2 + padding, y: btnHeight + padding }));
    var shootRect = new Kinetic.Rect($.extend(btnConfig, { x: btnWidth * 3 + padding * 3, y: btnHeight / 2 + padding, width: btnWidth * 2, height: btnHeight * 2 }));

    //Bind events
    var sendEvent = function (message) {
        PUBNUB.publish({
            channel: "surface_channel",
            message: {
                id: id,
                message: message
            }
        });
    };

    upRect.on("mousedown touchstart", function () { sendEvent("ud"); });
    downRect.on("mousedown touchstart", function () { sendEvent("dd"); });
    leftRect.on("mousedown touchstart", function () { sendEvent("ld"); });
    rightRect.on("mousedown touchstart", function () { sendEvent("rd"); });

    upRect.on("mouseup touchend", function () { sendEvent("uu"); });
    downRect.on("mouseup touchend", function () { sendEvent("du"); });
    leftRect.on("mouseup touchend", function () { sendEvent("lu"); });
    rightRect.on("mouseup touchend", function () { sendEvent("ru"); });

    controlLayer.add(upRect);
    controlLayer.add(downRect);
    controlLayer.add(leftRect);
    controlLayer.add(rightRect);
    controlLayer.add(shootRect);

    stage.add(bgLayer);
    stage.add(controlLayer);
};

$(document).ready(function () {
    var id = $("#clientId").val();
    var name = $("#clientName").val();

    drawControls(id);

    // LISTEN FOR MESSAGES
    PUBNUB.subscribe({
        channel: "client_channel_" + id,
        restore: false,
        callback: function (message) {
            switch (message.message) {
                case 'update':
                    $('.health-value').css('width', message.hp);
                    $('.health-text').text(message.hp);
                    break;
            }
        },
        connect: function () {
            PUBNUB.publish({
                channel: "surface_channel",
                message: {
                    id: id,
                    charName: name,
                    message: 'connected'
                }
            });
        }
    });


    $('.controls button').click(function () {
        PUBNUB.publish({
            channel: "surface_channel",
            message: {
                id: id,
                message: $(this).attr('ref')
            }
        });
    });
});