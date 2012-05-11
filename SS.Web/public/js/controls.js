(function() {
  var ControlLayer, Scene, drawScene, id, name, scene, timer;
  ControlLayer = (function() {
    ControlLayer.prototype._padding = 10;
    ControlLayer.prototype._activeFill = "#f8f8f8";
    ControlLayer.prototype._inactiveFill = "#f0f0f0";
    ControlLayer.prototype._currentDownEvents = [];
    ControlLayer.prototype._defaultButtonConfig = {
      fill: ControlLayer._inactiveFill,
      cornerRadius: 10,
      stroke: "#bbbbbb",
      strokeWidth: 1
    };
    ControlLayer.prototype.btnWidth = function(w) {
      return (w - (12 * this._padding)) / 5;
    };
    ControlLayer.prototype.btnHeight = function(h) {
      return (h - (2 * this._padding)) / 3;
    };
    function ControlLayer() {
      this.bgRect = new Kinetic.Rect({
        fill: "#eeeeee"
      });
      this.left = new Kinetic.Rect($.extend(this._defaultButtonConfig, {
        id: "l"
      }));
      this.right = new Kinetic.Rect($.extend(this._defaultButtonConfig, {
        id: "r"
      }));
      this.up = new Kinetic.Rect($.extend(this._defaultButtonConfig, {
        id: "u"
      }));
      this.down = new Kinetic.Rect($.extend(this._defaultButtonConfig, {
        id: "d"
      }));
      this.fire = new Kinetic.Rect($.extend(this._defaultButtonConfig, {
        id: "f"
      }));
      this.layer = new Kinetic.Layer;
      this.layer.add(this.bgRect);
      this.layer.add(this.left);
      this.layer.add(this.right);
      this.layer.add(this.up);
      this.layer.add(this.down);
      this.layer.add(this.fire);
      this.setEvents();
    }
    ControlLayer.prototype.setSize = function(width, height) {
      var dHeight, dWidth;
      dWidth = this.btnWidth(width);
      dHeight = this.btnHeight(height);
      this.bgRect.setSize(width, height);
      this.left.setSize(dWidth, dHeight);
      this.right.setSize(dWidth, dHeight);
      this.up.setSize(dWidth, dHeight);
      this.down.setSize(dWidth, dHeight);
      this.fire.setSize(dWidth * 2, dHeight * 2);
      this.left.setX(this._padding);
      this.left.setY(dHeight + this._padding);
      this.right.setX(dWidth * 2 + this._padding);
      this.right.setY(dHeight + this._padding);
      this.up.setX(dWidth + this._padding);
      this.up.setY(this._padding);
      this.down.setX(dWidth + this._padding);
      this.down.setY(dHeight * 2 + this._padding);
      this.fire.setX(dWidth * 3 + this._padding * 10);
      return this.fire.setY(dHeight / 2 + this._padding);
    };
    ControlLayer.prototype.setEvents = function() {
      var self;
      self = this;
      this.up.on("mousedown touchstart", function() {
        return self.buttonEvent(this, true);
      });
      this.down.on("mousedown touchstart", function() {
        return self.buttonEvent(this, true);
      });
      this.left.on("mousedown touchstart", function() {
        return self.buttonEvent(this, true);
      });
      this.right.on("mousedown touchstart", function() {
        return self.buttonEvent(this, true);
      });
      this.fire.on("mousedown touchstart", function() {
        return self.buttonEvent(this, true);
      });
      this.bgRect.on("mouseup touchend", function() {
        return self.endEvents();
      });
      this.up.on("mouseup touchend", function() {
        return self.endEvents();
      });
      this.down.on("mouseup touchend", function() {
        return self.endEvents();
      });
      this.left.on("mouseup touchend", function() {
        return self.endEvents();
      });
      this.right.on("mouseup touchend", function() {
        return self.endEvents();
      });
      return this.fire.on("mouseup touchend", function() {
        return self.endEvents();
      });
    };
    ControlLayer.prototype.buttonEvent = function(btn, isPressed) {
      var msg;
      if (isPressed === true) {
        this._currentDownEvents[btn.attrs.id] = true;
        btn.attrs.fill = this._activeFill;
        msg = btn.attrs.id + "d";
      } else {
        btn.attrs.fill = this._inactiveFill;
        msg = btn.attrs.id + "u";
      }
      return PUBNUB.publish({
        channel: "surface_channel",
        message: {
          id: id,
          message: msg
        }
      });
    };
    ControlLayer.prototype.endEvents = function() {
      var btn, evt, val, _ref;
      _ref = this._currentDownEvents;
      for (evt in _ref) {
        val = _ref[evt];
        btn = this.layer.get("#" + evt)[0];
        this.buttonEvent(btn, false);
      }
      return this._currentDownEvents = [];
    };
    return ControlLayer;
  })();
  Scene = (function() {
    function Scene(container) {
      this.container = container;
      this.stage = new Kinetic.Stage({
        container: this.container
      });
      this.controlLayer = new ControlLayer;
      this.stage.add(this.controlLayer.layer);
      this.resize();
    }
    Scene.prototype.resize = function() {
      var $doc, stageHeight, stageWidth;
      $doc = $(document);
      stageWidth = $doc.width();
      stageHeight = $doc.height();
      if (stageHeight > stageWidth) {
        $("#" + this.container).hide();
      } else {
        $("#" + this.container).show();
      }
      this.stage.setSize(stageWidth, stageHeight);
      return this.controlLayer.setSize(stageWidth, stageHeight);
    };
    Scene.prototype.draw = function() {
      var _ref;
      return (_ref = this.stage) != null ? _ref.draw() : void 0;
    };
    return Scene;
  })();
  id = $("#clientId").val();
  name = $("#clientName").val();
  scene = new Scene("container");
  drawScene = function() {
    return scene.draw();
  };
  timer = setInterval(drawScene, 16);
  $(window).on("resize", function() {
    return scene.resize();
  });
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
}).call(this);
