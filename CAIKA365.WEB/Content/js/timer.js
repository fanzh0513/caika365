!function (t, i) {
    var e = i.sendMsg;
    var n = i.bindMsg;
    var r = 0;
    function s(t, i) {
        return new o(t, i)
    }
    function o(t, i) {
        this._init(t, i)
    }
    o.format = function (t) {
        function i(t) {
            if (t < 10) return "0" + t;
            return t
        }
        return i(Math.floor(t / 60)) + ":" + i(t % 60)
    };
    o.prototype = {
        _init: function (t, e) {
            this.option = i.extend({
                progress: i.noop,
                countDown: true,
                endTime: 0,
                format: null
            },
            e);
            this.initTime = t;
            this.time = t;
            this.timeText = this.format(t);
            this.timerId = r++;
            this._run();
            this.status = "init";
            this.time_start = (new Date).getTime()
        },
        _progress: function (t, i) {
            var n = this.option;
            n.progress.apply(this, arguments);
            e("timer-" + this.timerId + ":whenLess");
            e("timer-" + this.timerId + ":whenMore");
            if (n.countDown && t <= n.endTime) this.stop();
            else if (!n.countDown && t >= n.endTime) this.stop()
        },
        start: function () {
            this.time_start = (new Date).getTime();
            if ("pause" == this.status) this.time_pause_duration += (new Date).getTime() - this.time_pause_start;
            if ("run" == this.status) return this;
            this.status = "run";
            this._run()
        },
        _run: function () {
            var t = this;
            clearTimeout(t.timer);
            var i = function () {
                if ("stop" == t.status) return;
                if (t.option.countDown) t.time = t.initTime - t._getDurationFromStart();
                else t.time = t.initTime + t._getDurationFromStart();
                t.timeText = t.format(t.time);
                t._progress(t.time, t.format(t.time));
                t.timer = setTimeout(i, 1e3)
            };
            setTimeout(i, 1e3)
        },
        _getDurationFromStart: function () {
            return Math.round(((new Date).getTime() - this.time_start) / 1e3)
        },
        stop: function (t) {
            this.initTime = t || this.initTime;
            this.status = "stop";
            e("timer-" + this.timerId + ":stop")
        },
        whenStop: function (t) {
            var i = this;
            n("timer-" + this.timerId + ":stop",
            function () {
                t.call(i, i.initTime, i.format(i.initTime))
            })
        },
        format: function (t) {
            var i = this.option.format || o.format;
            return i(t)
        },
        _when: function (t, i, e, r) {
            var s = this;
            if (!r) n("timer-" + this.timerId + ":" + t,
            function () {
                if ("whenLess" == t && s.time <= i || "whenMore" == t && s.time >= i) e.call(s, s.time, s.timeText)
            },
            this);
            else {
                var o = function () {
                    var n = false;
                    return function () {
                        if ("whenLess" == t && s.time <= i || "whenMore" == t && s.time >= i) {
                            if (n) return;
                            n = true;
                            e.call(s, s.time, s.timeText)
                        }
                    }
                }();
                n("timer-" + this.timerId + ":" + t, o, this)
            }
        },
        whenLess: function (t, i, e) {
            return this._when("whenLess", t, i, e)
        },
        whenMore: function (t, i, e) {
            return this._when("whenMore", t, i, e)
        }
    };
    i.Timer = o;
    i.timer = s
}(window, jQuery);
