class ControlLayer
    _padding: 10
    _activeFill: "#f8f8f8"
    _inactiveFill: "#f0f0f0"
    _currentDownEvents: []
    _defaultButtonConfig: {
        fill: this._inactiveFill
        cornerRadius: 10
        stroke: "#bbbbbb"
        strokeWidth: 1
    }
    btnWidth: (w) -> (w - (12 * this._padding)) / 5
    btnHeight: (h) -> (h - (2 * this._padding)) / 3
    constructor: ->
        this.bgRect = new Kinetic.Rect({ fill: "#eeeeee" })
        this.left = new Kinetic.Rect $.extend(this._defaultButtonConfig, { id: "l" })
        this.right = new Kinetic.Rect $.extend(this._defaultButtonConfig, { id: "r" })
        this.up = new Kinetic.Rect $.extend(this._defaultButtonConfig, { id: "u" })
        this.down = new Kinetic.Rect $.extend(this._defaultButtonConfig, { id: "d" })
        this.fire = new Kinetic.Rect $.extend(this._defaultButtonConfig, { id: "f" })
        this.layer = new Kinetic.Layer
        this.layer.add(this.bgRect)
        this.layer.add(this.left)
        this.layer.add(this.right)
        this.layer.add(this.up)
        this.layer.add(this.down)
        this.layer.add(this.fire)
        this.setEvents()
    setSize: (width, height) ->
        dWidth = this.btnWidth(width)
        dHeight = this.btnHeight(height)
        this.bgRect.setSize(width, height)
        this.left.setSize(dWidth, dHeight)
        this.right.setSize(dWidth, dHeight)
        this.up.setSize(dWidth, dHeight)
        this.down.setSize(dWidth, dHeight)
        this.fire.setSize(dWidth * 2, dHeight * 2)

        this.left.setX(this._padding)
        this.left.setY(dHeight + this._padding)
        this.right.setX(dWidth * 2 + this._padding)
        this.right.setY(dHeight + this._padding)
        this.up.setX(dWidth + this._padding)
        this.up.setY(this._padding)
        this.down.setX(dWidth + this._padding)
        this.down.setY(dHeight * 2 + this._padding)
        this.fire.setX(dWidth * 3 + this._padding * 10)
        this.fire.setY(dHeight / 2 + this._padding)
    setEvents: ->
        self = this
        this.up.on("mousedown touchstart", () -> self.buttonEvent(this, true))
        this.down.on("mousedown touchstart", () -> self.buttonEvent(this, true))
        this.left.on("mousedown touchstart", () -> self.buttonEvent(this, true))
        this.right.on("mousedown touchstart", () -> self.buttonEvent(this, true))
        this.fire.on("mousedown touchstart", () -> self.buttonEvent(this, true))
        
        this.bgRect.on("mouseup touchend", () -> self.endEvents())
        this.up.on("mouseup touchend", () -> self.endEvents())
        this.down.on("mouseup touchend", () -> self.endEvents())
        this.left.on("mouseup touchend", () -> self.endEvents())
        this.right.on("mouseup touchend", () -> self.endEvents())
        this.fire.on("mouseup touchend", () -> self.endEvents())
    buttonEvent: (btn, isPressed) ->
        if isPressed is true
            this._currentDownEvents[btn.attrs.id] = true
            btn.attrs.fill = this._activeFill
            msg = btn.attrs.id + "d"
        else
            btn.attrs.fill = this._inactiveFill
            msg = btn.attrs.id + "u"
        PUBNUB.publish({
            channel: "surface_channel"
            message: {
                id: id
                message: msg
            }
        })
    endEvents: ->
        for evt, val of this._currentDownEvents
            btn = this.layer.get("#" + evt)[0]
            this.buttonEvent(btn, false);
        this._currentDownEvents = [];

class Scene
    constructor: (@container) ->
        this.stage = new Kinetic.Stage { container: @container }
        this.controlLayer = new ControlLayer
        this.stage.add(this.controlLayer.layer)
        this.resize()
    resize: ->
        $doc = $(document);
        stageWidth = $doc.width();
        stageHeight = $doc.height();

        if stageHeight > stageWidth
            $("#" + this.container).hide();
        else
            $("#" + this.container).show();

        this.stage.setSize(stageWidth, stageHeight)
        this.controlLayer.setSize(stageWidth, stageHeight)
    draw: ->
        this.stage?.draw()

id = $("#clientId").val()
name = $("#clientName").val()
scene = new Scene "container"
drawScene = () -> scene.draw()
timer = setInterval(drawScene, 16)

$(window).on("resize", () -> scene.resize())

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