!function (e, s, a) {
    var t = {
        message: {
            ask: a.cdnUrl + "member/message_polling",
            sys: a.cdnUrl + "member/message_system",
            tag: a.cdnUrl + "member/mymessage_updateAllRead",
            url: "http://caipiao.163.com/my/message.html",
            set: "http://caipiao.163.com/my/message.html?set=yes"
        },
        interval: {
            ask: 3e4,
            sys: 18e4,
            close: 5e3
        }
    },
    i = {
        key: "cpMsgCache",
        getCache: function () {
            try {
                var s = e.LS.get(this.key) || "{}",
                t = /^\{.*\}$/.test(s) ? a.parseJSON(s) : {}
            } catch (i) {
                t = {}
            }
            return t
        },
        setCache: function (s) {
            s && e.LS.set(this.key, this.stringify(s))
        },
        clearCache: function () {
            e.LS.remove(this.key)
        },
        now: function (e) {
            var s = a.serverTime();
            return e ? +s : s
        },
        stringify: function (e) {
            if (JSON) return JSON.stringify(e);
            else return ""
        },
        act: function (e) {
            if (e && void 0 != e.news) e.news > 0 ? g(e) : m()
        }
    },
    n = 0,
    r = function () {
        var e = i.getCache(),
        s = i.now(true),
        n,
        r = e.time ? s - e.time : 0;
        if (r > 0 && r < t.interval.ask - 500) i.act(e.data);
        else {
            e.time = s;
            i.setCache(e);
            a.get(t.message.ask, function (s, a) {
                if (s) return;
                e.time = i.now(true);
                e.data = this.parseJSON(a);
                i.setCache(e);
                i.act(e.data)
            })
        }
        r = e.time2 ? s - e.time2 : 0;
        if (r <= 0 || r >= t.interval.sys - 500) {
            e.time2 = s;
            i.setCache(e);
            a.emptySendHttp(t.message.sys)
        }
    },
    c = '<div id="userMessagePopup"><div id="userMessageBox">' + '<div class="msgTitle">您有<a class="msgnew" target="_blank" href="{url}"></a>条<a class="msgbold" target="_blank" href="{url}">新消息</a><spap class="closeMsg">×</span></div>' + '<div class="msgContent">共有<a class="msgunread" target="_blank" href="{url}"></a>条未读消息<a href="javascript:void(0)" id="tagAllRead">全部标为已读</a> <a href="{set}" target="_blank">设置</a></div>' + "</div></div>",
    o = 0,
    g = function (a) {
        var i = s("#userMessagePopup"),
        n = s("#userMessageBox"),
        r;
        if (!i[0]) {
            if (a.recentUnreadMessageUrl) t.message.url = a.recentUnreadMessageUrl;
            s(document.body).append(s.format(c, s.extend({}, a, t.message)));
            i = s("#userMessagePopup").hide();
            i.find(".closeMsg").click(m);
            s("#tagAllRead").click(u);
            n = s("#userMessageBox").bind({
                mouseenter: function () {
                    o && e.clearTimeout(o)
                },
                mouseleave: function () {
                    o = e.setTimeout(m, t.interval.close)
                }
            }).hide()
        }
        r = i.find(".msgnew");
        r.text(a.news);
        i.find(".msgunread").text(a.unread);
        i.show();
        n.slideDown("fast")
    },
    m = function () {
        o && e.clearTimeout(o);
        o = 0;
        s("#userMessageBox").slideUp("fast", function () {
            s("#userMessagePopup").hide()
        })
    },
    u = function () {
        a.get(t.message.tag);
        s("#userMessagePopup").find(".msgnew,.msgunread").text(0);
        m();
        i.clearCache()
    };
    if (0 == document.URL.indexOf(t.message.url)) return;
    e.setInterval(r, t.interval.ask);
    r()
}(window, jQuery, Core);