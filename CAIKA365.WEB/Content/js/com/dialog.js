!function (t) {
    t(window, window.jQuery)
}(function (t, i, e) {
    var o = t.document,
    n = function () {
        var t = {},
        i = navigator.userAgent.toLowerCase(),
        e = /(webkit)[ \/]([\w.]+)/.exec(i) || /(opera)(?:.*version)?[ \/]([\w.]+)/.exec(i) || /(msie) ([\w.]+)/.exec(i) || /(trident)\/.+rv:([\w.]+)/.exec(i) || i.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+))?/.exec(i) || [];
        if (e[1]) {
            t[e[1]] = 1;
            t.version = e[2] || "0"
        }
        if (t.trident || t.msie) {
            t.msie = 1;
            t.version = o.documentMode || t.version
        }
        if (/qqbrowser\/(\d+)/.test(i)) {
            t.fucker = 1;
            t.qq = 1;
            t.version = RegExp.$1
        }
        t.version = "" + t.version;
        t.ver = +t.version.replace(/\..*$/, "");
        return t
    }(),
    a = function () {
        var t = o.createElement("div").style,
        n = "";
        if (t.transition !== e) return "transitionend";
        i.each({
            WebkitTransition: "webkitTransitionEnd",
            MozTransition: "transitionend",
            OTransition: "oTransitionEnd",
            msTransition: "MSTransitionEnd"
        },
        function (i, o) {
            if (t[i] !== e) n = o
        });
        return n
    }(),
    r = n.msie && n.ver < 7,
    s = !a || n.fucker || n.msie && n.ver < 10;
    if (!t.Class || !i) return;
    var l = {
        grayLayoutNum: 0,
        layoutNum: 0,
        prepareDoms: function (t, e, o) {
            var n = this.getMaxWH(),
            a = i("<div class='iDialogLayout'></div>").appendTo(this.body[0]).css("zIndex", t),
            s = "fixed" === a.css("position").toLowerCase(),
            l = r ? i("<iframe frameborder='0' class='iFrameGround fullFrameGround'/>").insertBefore(a).css("zIndex", t) : 0,
            c = 2 !== e && (1 !== e || this.grayLayoutNum > 0);
            e = {
                1: 1,
                2: 2
            }[e] || -1;
            if (c) a.addClass("iOpacityZero");
            else this.grayLayoutNum++;
            this.layoutNum++;
            if (o) a.addClass(o);
            s || a.height(n.height);
            if (l) {
                l.width(n.width).height(n.height);
                a.width(n.width);
                this.win.unbind("resize", this.resize).resize(this.resize)
            }
            return {
                layout: a,
                frame: l,
                opacityZero: c
            }
        },
        getMaxWH: function () {
            var t = this.body = this.body || i(o.body),
            e = this.win = this.win || i(e);
            return {
                width: Math.max(t.outerWidth(), o.documentElement.clientWidth),
                height: Math.max(t.outerHeight(), e.height(), o.documentElement.clientHeight)
            }
        },
        resize: function () {
            var t = l.getMaxWH();
            i(".fullFrameGround,.iDialogLayout").width(t.width).height(t.height)
        },
        animate: function (t, e, o, n) {
            if (1 == o && s) o = 0;
            if (1 == o) t[1 == e ? "hide" : "show"]()[1 == e ? "fadeIn" : "fadeOut"](300, n || i.noop);
            else (n || i.noop)()
        }
    };
    var c = Class.Base.Event.extend("Widgets.Layout", {
        init: function (i) {
            this.callSuper("onCreate onClick onDestroy");
            var e = this,
            o = l.prepareDoms(i.zindex || i.zIndex, i.type, i.css);
            this.options = i;
            this.opacityZero = o.opacityZero;
            this.cache = o;
            if (i.animate && !o.opacityZero) l.animate(o.layout, 1, i.animate);
            o.layout.mousedown(function (t) {
                e.trigger("onClick", o.layout, o.opacityZero)
            });
            t.setTimeout(function () {
                e.trigger("onCreate", o)
            },
            0)
        },
        destroy: function () {
            var t = this,
            o = t.options,
            n = t.cache.layout,
            a = t.cache.frame,
            r = function () {
                t.options = t.cache = e;
                n[0] && n.remove();
                a && a[0] && a.remove();
                if (0 === l.layoutNum) l.win.unbind("resize", l.resize);
                t.trigger("onDestroy")
            };
            this.destroy = i.noop;
            if (!this.opacityZero) l.grayLayoutNum--;
            l.layoutNum--;
            if (o.animate && !this.opacityZero && n[0]) l.animate(n, 2, o.animate, r);
            else r()
        }
    });
    var u = {
        title: null,
        content: "",
        type: "html",
        width: 0,
        height: 0,
        button: ["*确定"],
        css: "",
        position: "c",
        method: "prepend",
        dragable: 1,
        layout: 1,
        animate: 15,
        timeout: 0
    };
    var h = {
        guid: 0,
        cache: {},
        getCacheInfo: function () {
            var t = {
                dialogNum: 0,
                layoutNum: 0,
                opacityZeroLayout: 0
            };
            i.each(this.cache,
            function (i, e) {
                t.dialogNum++;
                if (e.layout) {
                    t.layoutNum++;
                    if (e.layout.opacityZero) t.opacityZeroLayout++
                }
            });
            return t
        },
        zindex: {
            model: 1e4,
            modeless: 1e3
        },
        checkOptions: function (t, o) {
            var n = i.extend({},
            u, t || {},
            i.isFunction(o) ? {
                callback: o
            } : {});
            if (i.isFunction(n.check)) {
                var a = n.check;
                n.check = function (t, i) {
                    return a.call(this.wrap[0], +t, i[0])
                }
            }
            if (i.isFunction(n.init)) {
                var r = n.init;
                n.init = function () {
                    return r.call(this.wrap[0])
                }
            }
            n.layout = function (t) {
                var i = {
                    "false": 0,
                    "true": 1,
                    "-1": -1,
                    0: 0,
                    1: 1,
                    2: 2
                }[t];
                return i === e ? -1 : i
            }(n.layout);
            n.animate = function (t) {
                var i = t,
                e = t;
                if (t > 2) {
                    i = parseInt(t / 10);
                    e = t - 10 * i
                }
                return s ? i : e
            }(n.animate);
            return n
        },
        bindParaEvent: function (t) {
            if (!t) return;
            var e = t.options;
            i.each({
                init: "onCreate",
                check: "onBtnClick",
                beforeClose: "onBeforeClose",
                callback: "onDestroy"
            },
            function (o, n) {
                i.isFunction(e[o]) && t.bind(n, e[o])
            })
        },
        prepareDoms: function (t, e) {
            var n = /^\*/,
            a = "",
            s = ['<div class="iDialog" style="visibility:hidden;z-index:', e.zindex, '" id="', t, '">', r && !e.layout ? '<iframe frameborder="0" class="iFrameGround"/>' : "", '<table class="iDialogWrapTable"><tr><td class="itd-top-left"></td><td class="itd-top-center"></td><td class="itd-top-right"></td></tr><tr><td class="itd-mid-left"></td><td class="itd-mid-center">', '<div class="iDialogContent">', '<div class="iDialogHead hide"><h1></h1></div><a class="iDialogClose hide" hidefocus="true" href="#"></a>', '<div class="iDialogBody"><div class="iDialogMain"></div></div>', !e.button || !e.button.length ? a : function () {
                var t = ['<div class="iDialogFoot">'],
                i = e.button,
                o = i.length,
                r = 0,
                s = 0;
                for (; r < o; r++) t[r + 1] = '<a href="javascript:;" rel="' + (o > 1 ? o - r - 1 : 1) + '" class="iDialogBtn' + (n.test(i[r]) && !s ? (s++, " focusBtn") : a) + '"><span>' + i[r].replace(n, "") + "</span></a>";
                t[o + 1] = "</div>";
                return t.join(a)
            }(), "</div>", '</td><td class="itd-mid-right"></td></tr><tr><td class="itd-bot-left"></td><td class="itd-bot-center"></td><td class="itd-bot-right"></td></tr></table>', "</div>"].join(a);
            i(o.body)[e.method](s);
            var l = i("#" + t).addClass(e.css || "");
            return {
                wrap: l,
                content: i(".iDialogContent", l),
                main: i(".iDialogMain", l),
                ground: i(".iFrameGround", l),
                closeBtn: i(".iDialogClose", l),
                title: i(".iDialogHead", l)
            }
        },
        markDom: function (t) {
            var e = i(t).eq(0),
            o = "posMark" + +new Date;
            if (!e[0]) return null;
            if (!e[0].orgPosMarkId) {
                e.after("<div style='display:none' id='" + o + "'/>");
                e[0].orgPosMarkId = o;
                e[0]._display = e.css("display")
            }
            return e.show()
        },
        revertDom: function (t) {
            var e = t.cache.orgDom;
            if (e) {
                e[0] && i("#" + e[0].orgPosMarkId).before(e.css("display", e[0]._display));
                delete t.cache.orgDom
            }
        },
        updateDom: function (t, i, e) {
            t.find("iframe").remove();
            t[i](e)
        },
        posReg: {
            l: /l/i,
            t: /t/i,
            r: /r/i,
            b: /b/i,
            c: /c/i
        },
        calPosition: function (e, o, n, a) {
            var r = {},
            s = "fixed" === a,
            l = i(t),
            c = [l.scrollLeft(), l.scrollTop()],
            u = [0, 0, l.width(), l.height()],
            h = {
                left: function () {
                    return s ? {
                        left: 0
                    } : {
                        left: u[0] + c[0]
                    }
                }(),
                center: function () {
                    return s ? {
                        left: "50%",
                        marginLeft: "-" + o / 2 + "px"
                    } : {
                        left: Math.floor((u[2] + u[0] - o) / 2) + c[0]
                    }
                }(),
                right: function () {
                    return s ? {
                        right: 0
                    } : {
                        left: u[2] - o + c[0]
                    }
                }()
            },
            d = {
                top: function () {
                    return s ? {
                        top: 0
                    } : {
                        top: u[1] + c[1]
                    }
                }(),
                center: function () {
                    var t = Math.floor((u[3] + u[1] - n) / 2) + c[1],
                    i = t < 0 ? Math.abs(t) : 0;
                    return s ? {
                        top: "50%",
                        marginTop: "-" + (n / 2 - i) + "px"
                    } : {
                        top: i ? 0 : t
                    }
                }(),
                bottom: function () {
                    return s ? {
                        bottom: 0
                    } : {
                        top: Math.max(0, u[3] - n + c[1])
                    }
                }()
            };
            switch (e.constructor) {
                case String:
                    var f = this.posReg;
                    e = 1 == e.length ? e + "c" : e;
                    e = f.t.test(e) ? f.l.test(e) ? 1 : f.r.test(e) ? 3 : 2 : f.b.test(e) ? f.l.test(e) ? 7 : f.r.test(e) ? 5 : 6 : f.c.test(e) ? f.l.test(e) ? 8 : f.r.test(e) ? 4 : 0 : 0;
                case Number:
                    if (e < 0 || e > 8) break;
                    var g = [[h.center, d.center], [h.left, d.top], [h.center, d.top], [h.right, d.top], [h.right, d.center], [h.right, d.bottom], [h.center, d.bottom], [h.left, d.bottom], [h.left, d.center]];
                    i.extend(r, g[e][0], g[e][1]);
                    break;
                case Array:
                    r.left = 0 === e[0] ? 0 : e[0] || "";
                    r.top = 0 === e[1] ? 0 : e[1] || "";
                    break;
                default:
                    r = i.extend({},
                    e || {})
            }
            i.each(["left", "right", "top", "bottom"],
            function (t, e) {
                var o = r[e],
                n = ["auto", "c", "center"];
                if (e in r && (o ? i.inArray(o, n) >= 0 : 0 !== o)) delete r[e]
            });
            if (!("left" in r) && !("right" in r)) i.extend(r, h.center);
            if (!("top" in r) && !("bottom" in r)) i.extend(r, d.center);
            return r
        },
        bindDrag: function (e, o) {
            var n = 2 == o ? e : i(".iDialogHead", e),
            a,
            r;
            if (!n[0] || !o && 0 !== o || "move" == n.css("cursor")) return;
            if (!i.fn.bindDrag) return "缺少拖动组件 $.fn.bindDrag，无法绑定拖动。";
            var s = "fixed" == e.css("position").toLowerCase(),
            l = s ?
            function (t) {
                return {
                    left: t[0].offsetLeft,
                    top: t[0].offsetTop
                }
            } : function (t) {
                return t.offset()
            };
            n.css("cursor", "move").bindDrag({
                beforeDrag: function (t) {
                    var e = i(t.target),
                    o = e[0].tagName.toLowerCase(),
                    a = e.closest("label", n),
                    r = true;
                    i.each("a button select input textarea object applet".split(" "),
                    function (t, i) {
                        if (o === i || e.closest(i, n)[0]) r = false
                    });
                    if (a[0] && a.attr("for")) r = false;
                    return r
                },
                dragStart: function (t) {
                    a = l(e);
                    r = [t.pageX, t.pageY];
                    e.css({
                        marginLeft: "",
                        marginTop: "",
                        left: a.left,
                        top: a.top,
                        right: "",
                        bottom: ""
                    });
                    i(".iDialogDragLayoutHelper", e).show()
                },
                onDrag: function (t) {
                    e.css({
                        left: a.left + t.pageX - r[0],
                        top: a.top + t.pageY - r[1]
                    })
                },
                dragEnd: function (o) {
                    var n = l(e),
                    a = i(t),
                    r = [a.width(), a.height()],
                    c = [e.width(), e.height()],
                    u = s ? [0, 0] : [a.scrollTop(), a.scrollLeft()],
                    h = {};
                    h.top = Math.max(u[0], Math.min(n.top, r[1] - 40 + u[0]));
                    h.left = Math.min(r[0] - 40 + u[1], Math.max(n.left, 60 - c[0] + u[1]));
                    if (h.top != n.top || h.left != n.left) e.animate(h, 200);
                    i(".iDialogDragLayoutHelper", e).hide()
                }
            })
        },
        unbindDrag: function (t) {
            if (!t) return;
            var i = t.wrap,
            e = t.options.dragable;
            if (i[0] && e) {
                (2 == e ? i : i.find(".iDialogHead")).unbind("mousedown").css("cursor", "");
                h.bindMoveTop(t)
            }
        },
        fixIframeGround: function (t) {
            var i = t ? t.cache.ground : 0;
            i && i.css("zIndex", -1).width(t.width()).height(t.height())
        },
        bindMoveTop: function (t) {
            if (!t || t.layout) return;
            t.wrap.mousedown(function () {
                h.moveTop(t)
            })
        },
        moveTop: function (t) {
            if (!t || t.layout || t.closing || t.closed) return;
            var e = t.options.zindex,
            o = 0;
            i.each(h.cache,
            function (t, i) {
                if (i && !i.closed && !i.closing && !i.layout) {
                    var n = i.options.zindex;
                    if (n < h.zindex.modeless && n > e) {
                        i.wrap.css("zIndex", i.options.zindex = n - 1);
                        o = n > o ? n : o
                    }
                }
            });
            o && t.wrap.css("zIndex", t.options.zindex = o)
        },
        findTopDialog: function () {
            var t = 0,
            e = null;
            i.each(this.cache,
            function (i, o) {
                if (o && !o.closing && !o.closed) {
                    var n = o.options.zindex;
                    e = n > t ? (t = n, o) : e
                }
            });
            return e
        },
        focusBtn: function (t) {
            t = t || h.findTopDialog();
            if (!t || o.hasFocus && !o.hasFocus()) return;
            i(".iDialogBtn[rel=1]", t.wrap).focus();
            i(".focusBtn:visible", t.wrap).focus()
        },
        closeTopDialog: function (t) {
            if (t && 27 == t.keyCode) {
                var e = h.findTopDialog();
                if (e && i(".iDialogClose:visible", e.wrap).length) e.close()
            }
        },
        checkShortCut: function () {
            i(o).unbind("keydown", this.closeTopDialog);
            for (var t in this.cache) {
                i(o).keydown(this.closeTopDialog);
                return
            }
        },
        bindBtnClick: function (t) {
            var o = t.wrap;
            if (o[0] && !o[0].initButtonClick) {
                o.delegate(".iDialogBtn", "click", function (o) {
                    var n = parseInt(i(this).attr("rel") || "-1", 10),
                    a = t.trigger("onBtnClick", n, i(this));
                    a = a === e ? n : a;
                    if (false !== a) t.close(a);
                    o.preventDefault()
                }).delegate(".iDialogClose", "click", function (i) {
                    t.close(null);
                    i.preventDefault()
                });
                o[0].initButtonClick = true
            }
        },
        animate: function (e, o, n, r) {
            var l, c, u = ["iDialogShowAniCore", "iDialogHideAniCore"][o - 1],
            h = r || i.noop,
            d = function () {
                e.removeClass(u)[1 == o ? "show" : "hide"]();
                h()
            };
            e.css("visibility", "visible");
            if (!n || n > 2 && s) {
                e[1 == o ? "show" : "hide"]();
                return h()
            }
            if (n < 3) {
                1 == o ? e.hide()[1 == n ? "fadeIn" : "slideDown"](200, h) : e.show()[1 == n ? "fadeOut" : "slideUp"](200, h);
                return
            }
            l = "iDialogAnimate" + n;
            if (1 == o) {
                e.one(a, d).addClass(l);
                t.setTimeout(function () {
                    e.addClass(u).removeClass(l)
                },
                10)
            } else {
                e.one(a, d).addClass(u);
                t.setTimeout(function () {
                    e.addClass(l)
                },
                10)
            }
        },
        closeAll: function () {
            i.each(h.cache,
            function (t, i) {
                i.close()
            })
        }
    };
    var d = Class.Base.Event.extend("Widgets.Dialog", {
        init: function (e, o) {
            this.callSuper("onCreate onShow onBeforeClose onClose onDestroy onBtnClick");
            var n = h.checkOptions(e || {},
            o),
            a = this;
            if (n.layout) {
                this.layout = c.create({
                    zindex: h.zindex.model++,
                    type: n.layout,
                    animate: n.animate ? 1 : 0
                }).onDestroy(function () {
                    a.layout = null
                }).onClick(function () {
                    if (this.opacityZero) a.flash()
                });
                n.zindex = h.zindex.model++
            } else {
                this.layout = null;
                n.zindex = h.zindex.modeless++
            }
            this.id = "iDialog" + h.guid++;
            this.cache = h.prepareDoms(this.id, n);
            this.options = n;
            this.returnValue = null;
            this.wrap = this.cache.wrap.css("visibility", "visible");
            this.title(true);
            this.width(n.width).height(n.height);
            this.content(n.content, n.type, true);
            this.wrap.css("visibility", "hidden");
            h.bindMoveTop(this);
            h.focusBtn(this);
            h.bindParaEvent(this);
            n.dragable && this.bindDrag();
            n.timeout && t.setTimeout(function () {
                a.close()
            },
            n.timeout);
            h.cache[this.id] = this;
            h.checkShortCut();
            t.setTimeout(function () {
                a.trigger("onCreate", a);
                h.animate(a.wrap, 1, n.animate,
                function () {
                    a.trigger("onShow", a);
                    i.sendMsg(50, "dialog.show", h.getCacheInfo())
                })
            },
            0)
        },
        flash: function () {
            var i = this.wrap,
            e = 0,
            o = 2 * 6,
            n = t.setInterval(function () {
                i[e % 2 ? "removeClass" : "addClass"]("iDialogFlash"); ++e >= o && t.clearInterval(n)
            },
            100);
            return this
        },
        content: function (t, e, o) {
            if ("boolean" === typeof e) {
                o = e;
                e = "html"
            }
            e = e || "html";
            if (!t && "" !== t) return this;
            var n = this.cache,
            a = n.wrap,
            r = this,
            s = h.updateDom,
            l;
            h.revertDom(this);
            switch (e) {
                case "html":
                case "text":
                    s(n.main || a, e, t);
                    break;
                case "shell":
                    n.main = n.content = null;
                    if (n.ground[0]) l = n.ground.clone();
                    s(a, "html", t);
                    break;
                case "insert":
                case "agent":
                    var c = h.markDom(t);
                    if (!c) this.content("没有找到页面内容（" + t + "）请检查！");
                    else {
                        if ("agent" == e || !n.main) this.content(c, "shell");
                        else s(n.main, "html", c);
                        n.orgDom = c
                    }
            }
            if (a.find("iframe:visible").length && !a.find(".iDialogDragLayoutHelper").length) a.append('<div class="iDialogDragLayoutHelper"></div>');
            if (l) {
                a.prepend(l);
                n.ground = l;
                h.fixIframeGround(this)
            }
            this.button = i(".iDialogBtn", a);
            try {
                this.button.setControlEffect("iDialogBtnDown");
                this.cache.closeBtn.setControlEffect("iDialogCloseDown")
            } catch (u) { }
            h.bindBtnClick(this);
            o && this.position(true);
            return this
        },
        position: function (t) {
            if (t === e) return this.options.position;
            if (true === t) t = this.options.position;
            this.options.position = t;
            this.wrap.css(i.extend({
                left: "",
                top: "",
                bottom: "",
                right: "",
                marginTop: "",
                marginLeft: ""
            },
            h.calPosition(t || 0, this.width(), this.height(), this.wrap.css("position"))));
            return this
        },
        unbindDrag: function () {
            h.unbindDrag(this);
            return this
        },
        bindDrag: function (t) {
            var i = this.options;
            t = t || i.dragable || 1;
            this.unbindDrag();
            var e = h.bindDrag(this.wrap, t);
            if (e) this.warn(e);
            i.dragable = t;
            return this
        },
        title: function (t) {
            if (t === e) return this.options.title;
            if (true === t) t = this.options.title;
            var i = this.cache,
            o = "iDialogNoTitle";
            if (i.title && i.title[0]) if ("" === t || null === t) {
                i.title.addClass("hide");
                i.closeBtn[null === t ? "addClass" : "removeClass"]("hide");
                this.wrap.addClass(o)
            } else {
                i.title.removeClass("hide").find("h1").html(t);
                i.closeBtn.removeClass("hide");
                this.wrap.removeClass(o)
            }
            this.options.title = t;
            return this
        },
        height: function (t) {
            var i = this.cache,
            o = i.wrap.height(),
            n = i.main,
            a = i.wrap;
            if (t === e) return o;
            t = parseInt(t, 10) || 0;
            if (0 === t) (n || a).css("height", "auto");
            else if (t) if (n) n.height(Math.max(t - (o - n.height()), 50));
            else a.height(Math.max(50, t));
            h.fixIframeGround(this);
            return this
        },
        width: function (t) {
            var i = this.cache;
            if (t === e) return i.wrap.width() || i.wrap.width();
            t = parseInt(t, 10) || 0;
            if (0 === t) i.wrap.css("width", "auto").addClass("autoWidthDialog");
            else if (t) i.wrap.removeClass("autoWidthDialog").width(Math.max(t, 200));
            h.fixIframeGround(this);
            return this
        },
        close: function (t) {
            if (this.closing || this.closed) return;
            var n = t === e ? this.returnValue : t,
            a = this;
            if (false === this.trigger("onBeforeClose", n)) return;
            this.returnValue = n;
            this.closing = true;
            h.checkShortCut();
            this.layout && this.layout.destroy();
            h.animate(this.wrap, 2, this.options.animate, function () {
                a.closed = true;
                a.wrap.hide();
                a.trigger("onClose", n);
                h.revertDom(a);
                h.updateDom(a.wrap, "html", "");
                a.wrap.empty().remove();
                delete h.cache[a.id];
                delete a.cache;
                delete a.wrap;
                delete a.button;
                delete a.options;
                if (r) o.body.className = o.body.className;
                a.trigger("onDestroy", n);
                i.sendMsg(50, "dialog.hide", h.getCacheInfo());
                h.focusBtn()
            })
        },
        toString: function () {
            return this.id
        }
    });
    d.prototype.destroy = d.prototype.close;
    d.closeAll = d.destroyAll = h.closeAll;
    i.bindMsg("dialog.show", function (t) {
        i.sendMsg("dialog.change", 1, t)
    });
    i.bindMsg("dialog.hide", function (t) {
        i.sendMsg("dialog.change", 0, t)
    });
    i.dialog = function (t, o) {
        if (t === e) return d.closeAll();
        if (t.constructor === d || "string" == typeof t && h.cache[t]) {
            (t.close ? t : h.cache[t]).close(o);
            return
        }
        if ("string" === typeof t) {
            if (/^iDialog\d+$/.test(t)) return;
            var n = 0 === t.indexOf(".") ? t.substr(1) : "";
            if (n) {
                i.each(h.cache,
                function (t, i) {
                    if (i.wrap.hasClass(n)) i.close(o)
                });
                return
            } else t = {
                content: t
            }
        }
        return d.create(t, o)
    };
    i.each({
        alert: function (t, i, e) {
            return d.info(t, i, e, 0, "iDialogAlert")
        },
        confirm: function (t, i, e) {
            return d.info(t, i, e, ["*确定", "取消"], "iDialogConfirm")
        },
        error: function (t, i, e) {
            return d.info(t, i, e, 0, "iDialogError")
        },
        info: function (t, e, o, n, a) {
            if (i.isFunction(e)) {
                o = e;
                e = 0
            }
            var r = e || n || ["*确定"];
            return d.create({
                title: null,
                css: a || "iDialogInfo",
                content: t,
                dragable: 2,
                button: r
            },
            o)
        },
        toast: function (t, e, o) {
            if (i.isFunction(e)) {
                o = e;
                e = 0
            }
            return d.create({
                title: null,
                css: "iDialogToast",
                content: t,
                button: [],
                layout: -1,
                position: {
                    bottom: "30%"
                },
                timeout: e || 2500,
                animate: 16
            },
            o).onCreate(function () {
                this.layout.destroy()
            })
        }
    },
    function (t, e) {
        d[t] = i.dialog[t] = e
    })
});
