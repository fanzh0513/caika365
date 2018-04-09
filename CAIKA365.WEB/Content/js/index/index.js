!function (t, i, e, a) {
    var n;
    e.extend(i, {
        loadIndexAD: function () {
            var t = i.cdnUrl + "home/carousel/index",
            a = e("#cpIndexAdBox"),
            n = a.find(".loadingData").removeClass("hide"),
            o = a.find(".loadedErr").addClass("hide"),
            r = a.find(".adList").addClass("hide"),
            s = a.find(".ctrol"),
            l = "[{img:'" + i.cdnUrl + "content/css/imgs/index/headFigureDefault.jpg" + "',title:'积分兑换彩金，购彩更省钱',url:'http://caipiao.163.com/sale/coupon_saleCouponIn.html'}]";
            var u = function (t) {
                n.addClass("hide");
                o.addClass("hide");
                r.removeClass("hide");
                var l = i.parseJSON(t),
                u = [],
                c = l.length,
                d = 0,
                h,
                p = parseInt(+i.serverTime() / 6e4);
                e.each(l, function (t, i) {
                    i.img = e.addUrlPara(i.img, p);
                    u[t] = "<li><a href='" + i.url + "' ii='" + (t + 1) + "' target='_blank'><img src='" + i.img + "' alt='" + i.title + "' title='" + i.title + "'/></a></li>"
                });
                r[0].innerHTML = u.join("");
                u = [];
                for (; d < c; d++) {
                    h = l[d];
                    u[d] = "<a " + (0 == d ? "class='active'" : "") + " href='" + h.url + "' title='" + h.title + "' ii='" + (d + 1) + "' target='_blank'>" + (d + 1) + "</a>"
                }
                s[l.length < 2 ? "hide" : "show"]().html(u.join(""));
                r.length && r.children().length > 1 && a.carousel({
                    list: r,
                    goBtnsWrap: s,
                    speed: 500
                })
            };
            this.isStatic ? u(l) : this.shareGet("indexAD", 60 * 1e3, t, function (t, i) {
                if (t) i = l;
                u(i)
            });
            a.delegate(".ctrol a,.adList a", "click", function () {
                var t = this.href,
                a = this.title,
                n = e(this).attr("ii"),
                o = "http://caipiao.163.com/help/special/link" + n + "/";
            })
        },
        quickInit: function () {
            e(".home-cz-cloumnav").bindTab(e.noop, "click", "li[rel]", "current");
            e("#ranksTab").bindTab(e.noop, "click", "dd");
            e("#loginBtn").bind({
                click: function (m) {
                    Core.easyNav.login();
                }
            });
            e("#regBtn").bind({
                click: function (m) {
                    Core.easyNav.reg();
                }
            });
            if (e.isIE678) e("#lotteryList")[0].className = "";
            if (e.isIE6) {
                var a = i.serverTime(),
                n = a.getFullYear() + "A" + a.getMonth() + "A" + a.getDate();
                if (t.LS.get("ie6update") != n) {
                    e("#topNav").after("<div id='ie6update'><div>温馨提示：您的浏览器版本过旧，打开网页速度缓慢。为了提高您的使用体验，建议您 <a href='http://windows.microsoft.com/zh-CN/internet-explorer/products/ie/home' target='_blank'>升级IE浏览器</a>。<a href='javascript:;' class='closeIE6update' title='今天不再提示'>x</a></div></div>");
                    e("#ie6update a").click(function () {
                        t.LS.set("ie6update", n);
                        e("#ie6update").remove()
                    })
                }
            }
            e.bindMsg('login.success', function (v) {
                if (v.flag == 'y') {
                    e("#login-pnl").removeAttr("style");
                    e("#usr-pnl").attr('style', 'display:block');
                    e('a.nickname').html(v.nick);
                    e("#avail").html(v.available);
                    e("#freezed").html(v.freezed);
                    e('div.user-loginst').delegate('a', 'click', function () {
                        Core.userAct(e(this).attr('id'));
                    });
                }
            })
        }
    });
    i.initAwardBox = function () {
        var t = e.format,
        a = e("#highFrequenceAward"),
        n = e("#tplHighFrequenceAward").html(),
        o = "http://caipiao.163.com/getAwardWarn.html",
        r = [];
        i.loadCdnJS("content/js/timer.js", function () {
            if (e("#highFrequenceAward").length) c()
        });
        function s(t, i) {
            return new l(t, i)
        }
        function l(t, i) {
            this._init(t, i)
        }
        l.prototype = {
            _init: function (t, i) {
                i = e.extend({
                    loop: e.noop
                },
                i);
                this.option = i;
                this.dom = e(t);
                this.ul = this.dom.find(">ul");
                this.lis = this.ul.find(">li");
                this.isMouseOn = false;
                this.scrolled = 0;
                this._binds()
            },
            _binds: function () {
                var t = this;
                this.dom.mouseenter(function () {
                    t.isMouseOn = true
                });
                this.dom.mouseleave(function () {
                    t.isMouseOn = false
                })
            },
            _scrollTop: function (t, i) {
                var t = t || 1;
                var a = 0;
                var n = this.ul;
                var o = this.lis;
                var i = i || e.noop;
                for (var r = 0; r < t; r++) a += o.eq(r).outerHeight();
                n.animate({
                    "margin-top": "-=" + a + "px"
                },
                i)
            },
            scrollTop: function (t) {
                var i = this;
                var t = t || 1;
                this._scrollTop(t,
                function () {
                    i.ul.find("li").slice(0, t).appendTo(i.ul);
                    i.ul.css("margin-top", 0);
                    i.scrolled += t;
                    if (i.scrolled >= i.lis.length) {
                        i.scrolled = 0;
                        i.option.loop.call(i)
                    }
                })
            },
            autoScroll: function () {
                var t = this;
                if (this.auto) clearTimeout(this.auto);
                if ("stop" == this.status) return this;
                this.auto = setTimeout(function () {
                    if (!t.isMouseOn) t.scrollTop();
                    t.autoScroll()
                },
                3e3);
                return this
            },
            stopAuto: function () {
                if (this.auto) clearTimeout(this.auto);
                this.status = "stop"
            }
        };
        function u() {
            e.each(r, function () {
                this.stop()
            });
            r = [];
            c()
        }
        function c() {
            h(function () {
                i.leastTime = 0;
                if (e("#highFrequenceAward li").length <= 1);
                else i.mar = s(a, {
                    loop: function () { }
                }).autoScroll();
                d()
            })
        }
        function d() {
            var t = [];
            a.find(".js-time").each(function (a, n) {
                var o = e(this);
                t[a] = o;
                var s = e.timer(Math.round(Number(o.attr("data-timer")) / 1e3), {
                    progress: function (e, n) {
                        t[a].html(n);
                        if (e <= i.leastTime) {
                            i.mar && i.mar.stopAuto();
                            u()
                        }
                    }
                });
                r.push(s)
            })
        }
        function h(t) {
            i.get(o, function (i, o) {
                var r, s;
                if (!i) {
                    try {
                        s = e.parseJSON(o)
                    } catch (l) {
                        p()
                    }
                    if (s.awardWarn.isHF) {
                        m(a, n, s.awardWarn.HFData);
                        if (t) t()
                    } else p()
                } else p()
            })
        }
        function p() {
            f();
            a.hide();
            e("#promotion .js-other").show()
        }
        function f() {
            a.html(" ");
            a.unbind()
        }
        function m(i, a, n) {
            i.html(" ");
            var o = {
                jxd11: "http://caipiao.163.com/order/jx11xuan5/",
                d11: "http://caipiao.163.com/order/11xuan5/",
                gdd11: "http://caipiao.163.com/order/gd11xuan5/"
            };
            var r = e("<ul></ul>");
            e.each(n,
            function (i, n) {
                var s = "";
                var l = b(n.secsLeft);
                var u = e('<li class="clearfix">' + t(a, n) + "</li>");
                e.each(n.number.split(/[\s\|]/), function () {
                    s += '<em class="smallRedball">' + this + "</em>"
                });
                s = e(s);
                u.find(".js-balls").append(s);
                u.find(".js-time").html(l);
                u.find(".awardBox_Btn").attr("href", g(n));
                u.find("span[data-avgmiss]").html(function () {
                    var t = e(this).attr("data-avgmiss");
                    t = Number(t) || 0;
                    return Math.round(10 * t) / 10
                });
                u.find(".gameEn").parent().attr("href", o[n.gameEn]);
                u.find(".js-playType").attr("href", o[n.gameEn] + "#betType=" + v(n.ruleDesc));
                u.appendTo(r)
            });
            i.append(r)
        }
        function g(t) {
            var i = t.beturl;
            var a = t.number + e.format("[{0}]", [o()]);
            var n = e.format("#betType={0}", v([t.ruleDesc]));
            return encodeURI(i + "?stakeNumber=" + a + "&activityType=28") + n;
            function o() {
                return t.playType.replace("组选", "")
            }
        }
        function v(t) {
            var i = {
                REN2: "rx2",
                REN3: "rx3",
                REN4: "rx4",
                REN5: "rx5",
                REN6: "rx6",
                REN7: "rx7",
                REN8: "rx8",
                QIAN1: "q1",
                QIAN2_ZHI: "q2",
                QIAN2_ZU: "q2_zu",
                QIAN3_ZHI: "q3",
                QIAN3_ZU: "q3_zu"
            };
            return i[t]
        }
        function b(t) {
            var i = t / 1e3;
            var e = Math.floor(i / 60);
            i = Math.floor(i % 60);
            return y(e) + ":" + y(i)
        }
        function y(t) {
            return t < 10 ? "0" + t : t.toString()
        }
    }
}(window, Core, jQuery);