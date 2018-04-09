!function(window, $, undefined) {
    !function() {
        try {
            window.document.execCommand("BackgroundImageCache", false, true)
        } catch(e) {}
        $.extend(Number.prototype, {
            Round: function(e, i) {
                var m = Math.pow(10, e || 0);
                return 0 == i ? Math.ceil(this * m) / m: Math.round(this * m + (5 - (i || 5)) / 10) / m
            },
            Cint: function(e) {
                return this.Round(0, e)
            },
            Round465: function(e) {
                var i, e = e || 0,
                m = "" + this,
                t = false,
                n;
                i = new RegExp("^(\\d*)(\\d)(\\.)(\\d{" + e + "})5(\\d*)$");
                if (i.test(m)) {
                    if (0 == e) {
                        m = m.replace(i, "$1$2");
                        n = RegExp.$2
                    } else {
                        m = m.replace(i, "$1$2$3$4");
                        n = RegExp.$4
                    }
                    if ( + RegExp.$5 > 0) t = true;
                    else if (n % 2 != 0) t = true;
                    if (t) m = +m + 1 / Math.pow(10, e)
                }
                m = ( + m).Round(e);
                return m
            }
        });
        var i = /./,
        m = i.compile && i.compile(i.source, "g");
        RegExp.regCompile = m;
        $.extend(String.prototype, {
            trim: function() {
                return this.replace(/^(?:\s|\xa0|\u3000)+|(?:\s|\xa0|\u3000)+$/g, "")
            },
            byteLen: function() {
                return this.replace(/([^\x00-\xff])/g, "ma").length
            },
            cutString: function(e, i) {
                var m = /([^\x00-\xff])/g,
                t = /([^\x00-\xff]) /g;
                if (i) {
                    var n = String(i),
                    s = n.length,
                    r = this.replace(m, "$1 ");
                    e = e >= s ? e - s: 0;
                    i = r.length > e ? n: "";
                    return r.substr(0, e).replace(t, "$1") + i
                }
                return this.substr(0, e).replace(m, "$1 ").substr(0, e).replace(t, "$1")
            }
        });
        $.fn.fixPosition = function() {
            var e = this,
            i, m, t, n, s = function(e, i) {
                var m = e[0].currentStyle[i];
                return m.indexOf("%") + 1 ? false: e.css(i).replace(/\D/g, "") || null
            },
            r = $(window),
            o,
            a,
            c,
            l,
            u;
            if ("absolute" == e.css("position")) {
                i = s(e, "top");
                m = s(e, "bottom");
                t = s(e, "left");
                n = s(e, "right");
                l = e.offsetParent()[0];
                u = l ? /^html|body$/i.test(l.tagName) : false;
                o = u ? +r.scrollTop() : 0;
                a = u ? +r.scrollLeft() : 0;
                c = function(s) {
                    var c = "resize" == s.type,
                    l;
                    if (c) {
                        l = e.is(":hidden");
                        if (!l) e.hide()
                    }
                    var h = +r.scrollTop(),
                    f = +r.scrollLeft();
                    if (u) m && e.css("bottom", +m + 1).css("bottom", m + "px");
                    else m && e.css("bottom", $(document).height() - r.height() - h + +m + "px");
                    i && e.css("top", +i + h - o + "px");
                    if (u) n && e.css("right", +n + 1).css("right", n + "px");
                    else n && e.css("right", $(document).width() - r.width() - f + +n + "px");
                    t && e.css("left", +t + f - a + "px");
                    if (c && !l) e.show()
                };
                r.scroll(c).resize(c)
            }
            return e
        }
    } ();
    !function() {
        $.isIE678 = eval('"\\v"=="v"');
        if ($.isIE678) {
            $.isIE8 = !!"1" [0];
            $.isIE6 = !$.isIE8 && (!document.documentMode || "BackCompat" == document.compatMode);
            $.isIE7 = !$.isIE8 && !$.isIE6;
            $.fn.extend({
                _bind_: $.fn.bind,
                bind: function(e, i, m) {
                    /^click$/gi.test(e) && d(this);
                    return this._bind_(e, i, m)
                }
            });
            var d = function(e) {
                var i = e.length,
                m = 0,
                t;
                for (; m < i; m++) {
                    t = e[m];
                    if (!t.fixClick) {
                        t.fixClick = true;
                        $(t).bind("dblclick",
                        function(e) {
                            var i = e.target,
                            m = 0;
                            while (i && 9 !== i.nodeType && (1 !== i.nodeType || i !== this)) {
                                if (1 === i.nodeType) if (i.fixClick) return;
                                i = i.parentNode
                            }
                            e.type = "click";
                            e.source = "dblclick";
                            $(e.target).trigger(e)
                        })
                    }
                }
            };
            var f = "abbr,article,aside,audio,canvas,datalist,details,dialog,eventsource,figure,footer,header,hgroup,mark,menu,meter,nav,output,progress,section,time,video".split(","),
            i = f.length;
            while (i--) document.createElement(f[i])
        }
    } ();
    $.extend({
        getUrlPara: function(e) {
            return $.getParaFromString(window.location.search.replace(/^\?/g, ""), e)
        },
        getHashPara: function(e) {
            var i = window.location.href.match(/#(.*)$/);
            return $.getParaFromString(i ? i[1] : "", e)
        },
        getPara: function(e) {
            return $.getUrlPara(e) || $.getHashPara(e)
        },
        getParaFromString: function(e, i) {
            var m = {};
            $.each(("" + e).match(/([^=&#\?]+)=[^&#]+/g) || [],
            function(e, i) {
                var t = i.split("="),
                n = decodeURIComponent(t[1]);
                if (m[t[0]] !== undefined) m[t[0]] += "," + n;
                else m[t[0]] = n
            });
            return i ? m[i] || "": m
        },
        safeHTML: function(e) {
            return String(e).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#39;")
        },
        safeRegStr: function(e) {
            return String(e).replace(/([\\\(\)\{\}\[\]\^\$\+\-\*\?\|])/g, "\\$1")
        },
        falseFn: function() {
            return false
        },
        stopProp: function(e) {
            e.stopPropagation()
        },
        preventDft: function(e) {
            e.preventDefault()
        },
        isLeftClick: function(e) {
            return e.button == (eval('"\\v"=="v"') ? 1 : 0)
        },
        addUrlPara: function(e, i, m) {
            var t = (e + "").split("#"),
            n;
            if (m) t[0] = $.removeUrlPara(t[0], $.map(i.match(/([^=&#\?]+)=[^&#]+/g),
            function(e) {
                return e.replace(/=.+$/, "")
            }));
            n = t[0].indexOf("?") + 1 ? "&": "?";
            return (t[0] + n + i + (t.length > 1 ? "#" + t[1] : "")).replace(/\?\&/, "?")
        },
        removeUrlPara: function(e, i) {
            var m = e.split("#"),
            t = m[0].split("?"),
            n = t[0],
            s = t.length > 1 ? t[1] : "",
            r = m.length > 1 ? "#" + m[1] : "",
            o = "string" === typeof i && i ? [i] : i.join ? i: [];
            if (!o.length || !s) return n.replace(/\?.+$/, "") + r;
            $.map(o,
            function(e) {
                return e.replace(/([\\\(\)\{\}\[\]\^\$\+\-\*\?\|])/g, "\\$1")
            });
            return (n + "?" + s.replace(new RegExp("(?:^|&)(?:" + o.join("|") + ")=[^&$]+", "g"), "").replace(/^&/, "")).replace(/\?$/, "") + r
        },
        fillUrl: function(e) {
            if ("string" !== typeof e || "" == e) return e;
            if (!/^http/i.test(e)) {
                var i = window.location.port || "80",
                m = /^\//.test(e);
                if (!m) e = document.URL.replace(/\/[^\/]*$/g, "/") + e;
                else e = window.location.protocol + "//" + window.location.host + ("80" == i ? "": ":" + i) + e
            }
            return e
        },
        addFav: window.sidebar && window.sidebar.addPanel ? function(e, i) {
            window.sidebar.addPanel(i, e, "")
        }: function(e, i) {
            try {
                window.external.addFavorite(e, i)
            } catch(m) {
                window.alert("请尝试点击 Ctrl + D 来添加！")
            }
        }, formatTime: function(e, i) {
            var m = /^\d+$/i.test(e + "") ? +e: Date.parse(e);
            if (isNaN(m)) return e;
            var t = new Date(m),
            n = function(e) {
                return ("0" + e).slice( - 2)
            },
            s = t.getFullYear(),
            r = t.getMonth() + 1,
            o = n(r),
            a = t.getDate(),
            c = n(a),
            l = t.getHours(),
            u = n(l),
            h = t.getMinutes(),
            f = n(h),
            W = t.getSeconds(),
            d = n(W);
            return (i || "yyyy-MM-dd hh:mm:ss").replace(/yyyy/g, s).replace(/MM/g, o).replace(/M/g, r).replace(/dd/g, c).replace(/d/g, a).replace(/hh/g, u).replace(/h/g, l).replace(/mm/g, f).replace(/m/g, h).replace(/ss/g, d).replace(/s/g, W)
        }
    });
    !function(e) {
        var i = {},
        m = {},
        t = 0,
        n = Object.prototype.toString,
        s = function(e, i) {
            var m = i || "%",
            t = new Function("var p=[],my=this,data=my,print=function(){p.push.apply(p,arguments);};p.push('" + e.replace(/[\r\t\n]/g, " ").split("<" + m).join("	").replace(new RegExp("((^|" + m + ">)[^\\t]*)'", "g"), "$1\r").replace(new RegExp("\\t=(.*?)" + m + ">", "g"), "',$1,'").split("	").join("');").split(m + ">").join("p.push('").split("\r").join("\\'") + "');return p.join('');");
            return t
        };
        e.template = function(e, r, o) {
            o = o || "%";
            var a = "[object Function]" === n.call(e) ? e: !/\W/.test(e) ? m[e + o] = m[e + o] || s(document.getElementById(e).innerHTML, o) : function() {
                for (var n in i) if (i[n] === e) return m[n];
                return i[++t] = e,
                m[t] = s(e, o)
            } ();
            return r ? a.call(r) : a
        }
    } (window.jQuery || window);
    $.fn.extend({
        disabled: function(e) {
            return this.each(function() {
                var i = this.bindDownCssFix || "",
                m = !e ? "disabled" + i: e;
                $(this).attr("disabled", "disabled").addClass(m)[0].disabled = true
            })
        },
        enabled: function(e) {
            return this.each(function() {
                var i = this.bindDownCssFix || "",
                m = !e ? "disabled" + i: e;
                $(this).removeClass(m).removeAttr("disabled")[0].disabled = false
            })
        },
        disableDrag: function() {
            return this.bind("dragstart", $.falseFn)
        },
        enableDrag: function() {
            return this.unbind("dragstart", $.falseFn)
        }
    });
    !function() {
        var e = RegExp.regCompile ? /./.compile("\\{([\\w\\.]+)\\}", "g") : /\{([\w\.]+)\}/g;
        $.format = function(i, m) {
            var t = true,
            n, s, r = m === undefined ? null: $.isPlainObject(m) ? (t = false, m) : $.isArray(m) ? m: Array.prototype.slice.call(arguments, 1);
            if (null === r) return i;
            n = t ? r.length: 0;
            s = RegExp.regCompile ? /./.compile("^\\d+$") : /^\d+$/;
            return String(i).replace(e, function(e, i) {
                var m = s.test(i),
                o,
                a,
                c;
                if (m && t) {
                    o = parseInt(i, 10);
                    return o < n ? r[o] : e
                } else {
                    a = i.split(".");
                    c = r;
                    for (var l = 0; l < a.length; l++) c = c[a[l]];
                    return c === undefined ? e: c
                }
            })
        }
    } ();
    $.fn.bindTab = function(e, i, m, t, n) {
        if (!$.isFunction(e)) {
            n = t;
            t = m;
            m = i;
            i = e;
            e = $.noop
        }
        return this.each(function() {
            var s = $(this),
            r,
            o = (t || "active").split("::"),
            a = m || "li",
            c = n || "rel",
            l = i || "mouseenter",
            u = "mouseenter" == l,
            h = function(i) {
                var m = $(s.find("." + o[0]).removeClass(o[0]).attr(c)),
                t = $(i.addClass(o[0]).attr(c));
                if (o[1]) {
                    m.addClass(o[1]);
                    t.removeClass(o[1])
                } else {
                    m.hide();
                    t.show()
                }
                e.call(i[0], t[0])
            };
            s.delegate(a, l, function() {
                var e = $(this);
                if (e.hasClass(o[0]) || this.disabled) return;
                if (u) {
                    r && window.clearTimeout(r);
                    r = window.setTimeout(function() {
                        h(e)
                    },
                    200)
                } else h(e)
            });
            u && s.delegate(a, "mouseleave", function() {
                r && window.clearTimeout(r);
                r = 0
            });
            "a" == a && s.delegate(a, "click", function(e) {
                e.preventDefault()
            });
            var f = s.find("." + o[0]);
            if (!f[0]) s.find(a).eq(0).addClass(o[0]);
            s.find(a).each(function() {
                var e = $($(this).attr(c)),
                i = $(this).hasClass(o[0]);
                if (o[1]) e[i ? "removeClass": "addClass"](o[1]);
                else e[i ? "show": "hide"]()
            })
        })
    };
    !function(e) {
        if (isNaN(new Date("2013-12-09T08:39:15"))) Date.prototype.toJSON = function() {
            var e = function(e) {
                return ("0" + e).slice( - 2)
            };
            return this.getFullYear() + "/" + e(this.getMonth() + 1) + "/" + e(this.getDate()) + " " + e(this.getHours()) + ":" + e(this.getMinutes()) + ":" + e(this.getSeconds())
        };
        if (e.JSON) return;
        var i = {
            "\b": "\\b",
            "	": "\\t",
            "\n": "\\n",
            "\f": "\\f",
            "\r": "\\r",
            '"': '\\"',
            "\\": "\\\\"
        },
        m = function(e) {
            if (/["\\\x00-\x1f]/.test(e)) e = e.replace(/["\\\x00-\x1f]/g,
            function(e) {
                var m = i[e];
                if (m) return m;
                m = e.charCodeAt();
                return "\\u00" + Math.floor(m / 16).toString(16) + (m % 16).toString(16)
            });
            return '"' + e + '"'
        },
        t = function(e) {
            var i = ["["],
            m = e.length,
            t,
            n,
            s;
            for (n = 0; n < m; n++) {
                s = e[n];
                switch (typeof s) {
                    case "undefined":
                    case "function":
                    case "unknown":
                        break;
                    default:
                        if (t) i.push(",");
                        i.push(a(s));
                        t = 1
                }
            }
            i.push("]");
            return i.join("")
        },
        n = function(e) {
            return e < 10 ? "0" + e: e
        },
        s = function(e) {
            if (e.toJSON) return '"' + e.toJSON() + '"';
            return '"' + e.getUTCFullYear() + "-" + n(e.getUTCMonth() + 1) + "-" + n(e.getUTCDate()) + "T" + n(e.getUTCHours()) + ":" + n(e.getUTCMinutes()) + ":" + n(e.getUTCSeconds()) + '"'
        },
        r = Object.prototype.hasOwnProperty,
        o = function(e) {
            var i = ["{"],
            m,
            t;
            for (var n in e) if (r.call(e, n)) {
                t = e[n];
                switch (typeof t) {
                    case "undefined":
                    case "unknown":
                    case "function":
                        break;
                    default:
                        m && i.push(",");
                        m = 1;
                        i.push(a(n) + ":" + a(t))
                }
            }
            i.push("}");
            return i.join("")
        },
        a = function(e) {
            switch (typeof e) {
                case "unknown":
                case "function":
                case "undefined":
                    return;
                case "number":
                    return isFinite(e) ? String(e) : "null";
                case "string":
                    return m(e);
                case "boolean":
                    return String(e);
                default:
                    return null === e ? "null": e instanceof Array ? t(e) : e instanceof Date ? s(e) : o(e)
            }
        };
        e.JSON = {
            parse: function(e) {
                e = e.replace(/("|')\\?\/Date\((-?[0-9+]+)\)\\?\/\1/g, "new Date($2)");
                return new Function("return " + e)()
            },
            stringify: function(e) {
                return a(e)
            }
        }
    } (window);
    !function(e) {
        var i, m = function() {},
        t = e.document,
        n = {
            set: m,
            get: m,
            remove: m,
            clear: m,
            each: m,
            obj: m
        }; !
        function() {
            if ("localStorage" in e) try {
                i = e.localStorage;
                return
            } catch(m) {}
            var n = t.getElementsByTagName("head")[0],
            s = e.location.hostname || "localStorage",
            r = new Date,
            o,
            a;
            if (!n.addBehavior) {
                try {
                    i = e.localStorage
                } catch(m) {
                    i = null
                }
                return
            }
            try {
                a = new ActiveXObject("htmlfile");
                a.open();
                a.write("<s" + "cript>document.w=window;</s" + 'cript><iframe src="/favicon.ico"></frame>');
                a.close();
                o = a.w.frames[0].document;
                n = o.createElement("head");
                o.appendChild(n)
            } catch(m) {
                n = t.getElementsByTagName("head")[0]
            }
            try {
                r.setDate(r.getDate() + 36500);
                n.addBehavior("#default#userData");
                n.expires = r.toUTCString();
                n.load(s);
                n.save(s)
            } catch(m) {
                return
            }
            var c, l;
            try {
                c = n.XMLDocument.documentElement;
                l = c.attributes
            } catch(m) {
                return
            }
            var u = "p__hack_",
            h = "m-_-c",
            f = new RegExp("^" + u),
            W = new RegExp(h, "g"),
            d = function(e) {
                return encodeURIComponent(u + e).replace(/%/g, h)
            },
            p = function(e) {
                return decodeURIComponent(e.replace(W, "%")).replace(f, "")
            };
            i = {
                length: l.length,
                isVirtualObject: true,
                getItem: function(e) {
                    return (l.getNamedItem(d(e)) || {
                        nodeValue: null
                    }).nodeValue || c.getAttribute(d(e))
                },
                setItem: function(e, i) {
                    try {
                        c.setAttribute(d(e), i);
                        n.save(s);
                        this.length = l.length
                    } catch(m) {}
                },
                removeItem: function(e) {
                    try {
                        c.removeAttribute(d(e));
                        n.save(s);
                        this.length = l.length
                    } catch(i) {}
                },
                clear: function() {
                    while (l.length) this.removeItem(l[0].nodeName);
                    this.length = 0
                },
                key: function(e) {
                    return l[e] ? p(l[e].nodeName) : undefined
                }
            };
            if (! ("localStorage" in e)) e.localStorage = i
        } ();
        e.LS = !i ? n: {
            set: function(e, m) {
                if (this.get(e) !== undefined) this.remove(e);
                i.setItem(e, m)
            },
            get: function(e) {
                var m = i.getItem(e);
                return null === m ? undefined: m
            },
            remove: function(e) {
                i.removeItem(e)
            },
            clear: function() {
                i.clear()
            },
            each: function(e) {
                var i = this.obj(),
                m = e ||
                function() {},
                t;
                for (t in i) if (false === m.call(this, t, this.get(t))) break
            },
            obj: function() {
                var e = {},
                m = 0,
                t, n;
                if (i.isVirtualObject) e = i.key( - 1);
                else {
                    t = i.length;
                    for (; m < t; m++) {
                        n = i.key(m);
                        e[n] = this.get(n)
                    }
                }
                return e
            }
        };
        if (e.jQuery) e.jQuery.LS = e.LS
    } (window);
    $.hash = function(e, i) {
        if ("string" === typeof e && i === undefined) return $.getHashPara(e);
        var m = window.location.hash.replace(/^#*/, "").split("&"),
        t = {},
        n = m.length,
        s = 0,
        r,
        o = {},
        a = {},
        c,
        l;
        for (; s < n; s++) {
            r = m[s].split("=");
            if (2 == r.length && r[0].length) {
                l = decodeURIComponent(r[0]);
                c = l.toLowerCase();
                if (!a[c]) {
                    o[l] = decodeURIComponent(r[1]);
                    a[c] = l
                }
            }
        }
        if (e === undefined) return o;
        if ($.isPlainObject(e)) t = e;
        else t[e] = i;
        for (l in t) {
            i = t[l];
            c = l.toLowerCase();
            a[c] && o[a[c]] !== undefined && delete o[a[c]];
            if (null !== i) {
                a[c] = l;
                o[l] = String(i)
            }
        }
        m.length = 0;
        for (l in o) m.push(encodeURIComponent(l) + "=" + encodeURIComponent(o[l]));
        window.location.hash = "#" + m.join("&")
    };
    $.cookie = function(e, i, m) {
        if (arguments.length > 1 && (null === i || "object" !== typeof i)) {
            m = $.extend({},
            m);
            if (null === i) m.expires = -1;
            if ("number" === typeof m.expires) {
                var t = m.expires,
                n = m.expires = new Date;
                n.setDate(n.getDate() + t)
            }
            return document.cookie = [encodeURIComponent(e), "=", m.raw ? String(i) : encodeURIComponent(String(i)), m.expires ? "; expires=" + m.expires.toUTCString() : "", m.path ? "; path=" + m.path: "", m.domain ? "; domain=" + m.domain: "", m.secure ? "; secure": ""].join("")
        }
        m = i || {};
        var s, r = m.raw ?
        function(e) {
            return e
        }: decodeURIComponent;
        return (s = new RegExp("(?:^|; )" + encodeURIComponent(e) + "=([^;]*)").exec(document.cookie)) ? r(s[1]) : null
    };
    !function () {
        var e = "caika310.com",
        i = /\.caika310\.com$/i,
        m = function(e) {
            var m = (e + "").toLowerCase(),
            t = m.indexOf("http");
            return t < 0 ? i.test(m) ? m: "": t ? "": m.replace(/^https?:\/\//, "").replace(/\/.+$/, "")
        },
        t = {},
        n = {},
        s = function(e, i) {
            var r = m(e),
            o = window.location.host + "",
            a = t[r],
            c = e.replace(/\/$/g, "") + "/agent/ajaxAgentV2.htm",
            l = function(e) {
                var i = n[r] || [];
                $.each(i,
                function(i, m) {
                    m(e)
                });
                n[r] = null
            };
            if (c.indexOf("http") < 0) c = "http://" + c;
            if (!r || r == o) {
                l($);
                i($);
                return
            }
            if (a) try {
                a.__test = +new Date
            } catch(u) {
                t[r] = a = null
            }
            if (a) {
                l(a);
                i(a);
                return
            }
            if (n[r]) {
                n[r].push(i);
                return
            }
            if (!document.body) {
                window.setTimeout(function() {
                    s(e, i)
                }, 1);
                return
            }
            n[r] = n[r] || [];
            n[r].push(i);
            var h = document.createElement("iframe");
            h.src = "about:blank";
            h.width = 0;
            h.height = 0;
            h.setAttribute("frameborder", 0);
            h.scrolling = "no";
            document.body.appendChild(h);
            function f(e, i) {
                $(h).unbind().bind("load", function() {
                    try {
                        var m = h.contentWindow.jQuery;
                        m.__test = +new Date;
                        t[e] = m;
                        l(m)
                    } catch(n) {
                        i && i()
                    }
                });
                h.src = c + "?domain=" + e + "&v=" + +new Date
            }
            if (r.indexOf(document.domain) > 0) f(document.domain = document.domain);
            else f(document.domain, function() {
                if (r.indexOf(document.domain) > 0) f(document.domain);
                else f("")
            })
        },
        r = function(e) {
            e = e.replace(/("|')\\?\/Date\((-?[0-9+]+)\)\\?\/\1/g, "new Date($2)");
            return new Function("return " + e)()
        },
        o = {},
        a = function(e, t, n, r, o) {
            var a = window.location.host + "",
            l = m(t) || a,
            u = "http:",
            h = "80",
            f;
            if (/^(https?:)/i.test(t)) {
                u = RegExp.$1.toLowerCase();
                if (/:(\d+)/i.test(t)) h = RegExp.$1 || "80"
            } else {
                u = window.location.protocol;
                h = window.location.port || "80"
            }
            if (window.location.protocol != u || (window.location.port || "80") != h) {
                f = $.isFunction(r) ? r: $.isFunction(n) ? n: $.noop;
                f.call(window.Core || window, 2, "", "protocols or ports not match");
                return
            }
            if (i.test(l) && i.test(a) && l.indexOf(document.domain) >= 0 && "http:" == u) s(l, function(i) {
                c(i, e, t, n, r, o)
            });
            else c(jQuery, e, t.replace(/https?:\/\/[^\/]+/, ""), n, r, o)
        },
        n = {},
        c = function(e, i, m, t, s, a) {
            var c = $.isFunction(s) ? s: $.noop,
            l = m,
            u,
            h,
            f = window.Core || window,
            W = false,
            d = (l.indexOf("?") + 1 ? "&": "?") + "cache=" + +new Date,
            p,
            C;
            if ($.isFunction(t)) {
                c = t;
                t = null;
                a = s
            }
            if (a && 0 == a.indexOf("*")) {
                W = true;
                a = a.substr(1)
            }
            if (a) {
                if (0 === a.indexOf("!")) {
                    a = a.substr(1);
                    if (n[a]) {
                        n[a].push(c);
                        return
                    }
                    n[a] = [];
                    s = c;
                    c = function() {
                        var e = arguments,
                        i = this;
                        s.apply(i, e);
                        $.each(n[a], function(m, t) {
                            t.apply(i, e)
                        });
                        delete n[a]
                    }
                }
                u = o[a];
                if (u) {
                    if (0 !== a.indexOf("@")) return;
                    h = u.readyState;
                    if (h > 0 && h < 5) {
                        try {
                            u.aborted = true
                        } catch(g) {}
                        u.abort()
                    }
                }
            }
            p = i.split(".");
            C = p.length > 1 ? p[1] : "";
            u = e.ajax({
                url: l + (W ? "": d),
                type: p[0],
                data: t,
                success: function(e, i, m) {
                    delete o[a];
                    if (m.aborted) return;
                    e = m.responseText;
                    if (e == undefined || null == e || "" == e || e.indexOf("<!DOCTYPE") >= 0) {
                        c.call(f, 1, e, i);
                        return
                    }
                    if ("JSON" == C) try {
                        e = r(e)
                    } catch(t) {
                        c.call(f, 3, m.responseText, i);
                        return
                    }
                    c.call(f, 0, e, i)
                },
                error: function(e, i) {
                    delete o[a];
                    if (!i || "error" == i) {
                        c.call(f, 1, "", i);
                        return
                    }
                    if (e.aborted) return;
                    c.call(f, 1, e.responseText, i)
                }
            });
            a && (o[a] = u)
        };
        $.extend({
            get2: function(e, i, m, t) {
                a("GET", e, i, m, t);
                return this
            },
            post2: function(e, i, m, t) {
                a("POST", e, i, m, t);
                return this
            },
            getJSON2: function(e, i, m, t) {
                a("GET.JSON", e, i, m, t);
                return this
            },
            postJSON2: function(e, i, m, t) {
                a("POST.JSON", e, i, m, t);
                return this
            }
        })
    } ();
    $.bindModule = function(e, i, m) {
        if ("object" != typeof i) {
            m = i;
            i = e;
            e = 0
        }
        var t = e || this;
        $.each(i || {}, function(e, i) {
            i && i.js && $.each(e.split(" "), function(e, n) {
                if (t[n]) return;
                var s = [],
                r = [];
                var o = t[n] = function() {
                    var e = arguments;
                    s.push(this);
                    r.push(e);
                    if (1 == o.autoLoaded) return;
                    o.autoLoaded = 1;
                    var a = window.setTimeout(function() {
                        o.autoLoaded = 0
                    }, 1e3);
                    i.css && $.loadCss(i.css, m);
                    $.loadJS(i.js, function() {
                        a && window.clearTimeout(a);
                        if (t[n] === o) {
                            window.console && window.console.log("方法" + n + "在" + i.js + "中未被定义！自动加载模块处理失败！");
                            t[n] = $.noop;
                            return
                        }
                        for (var e = r.length, m = 0; m < e; m++)
                            t[n].apply(s[m], r[m]);
                        r.length = 0
                    }, m)
                }
            })
        });
        return this
    };
    !function() {
        var e = {},
        i = function(i, m, t, n, s) {
            var r = m.toLowerCase().replace(/#.*$/, "").replace("/?.*$/", ""),
            o,
            a,
            c = $.isFunction,
            l = e[r] || [],
            u = !!(t || $.noop)(m),
            h = window.CollectGarbage || $.noop;
            if (u) {
                c(n) && n();
                return
            }
            e[r] = l;
            if (!l || !l.loaded || t && !u) {
                c(n) && l.push(n);
                l.loaded = 1;
                o = document.createElement(i),
                a = document.getElementsByTagName("head")[0] || document.documentElement;
                m = m + (m.indexOf("?") >= 0 ? "&": "?") + (window.Core ? Core.version: +new Date);
                if ("link" == i) {
                    o.rel = "stylesheet";
                    o.type = "text/css";
                    o.media = "screen";
                    o.charset = s || "UTF-8";
                    o.href = m
                } else {
                    o.type = "text/javascript";
                    o.charset = s || "UTF-8";
                    var f = false;
                    o.onload = o.onreadystatechange = function() {
                        if (!f && (!this.readyState || {
                            loaded: 1,
                            complete: 1
                        } [this.readyState])) {
                            f = true;
                            o.onload = o.onreadystatechange = null;
                            this.parentNode.removeChild(this);
                            var i = e[r],
                            m = i.length,
                            t = 0;
                            i.loaded = 2;
                            for (; t < m; t++) c(i[t]) && i[t]();
                            i.length = 0;
                            i = a = o = null;
                            h()
                        }
                    };
                    o.src = m
                }
                a.appendChild(o, a.lastChild)
            } else if (2 == l.loaded) {
                c(n) && n();
                l = null;
                h()
            } else {
                c(n) && l.push(n);
                l = null;
                h()
            }
        },
        m = function(e, i) {
            if (!i) return e;
            return /^http/i.test(e) ? e: i.replace(/\/*$/, "") + (0 == e.indexOf("/") ? "": "/") + e
        };
        $.extend({
            loadJS: function (e, t, n, s, r) {
                if (!$.isFunction(n)) {
                    r = s;
                    s = n;
                    n = t;
                    t = null
                }
                if (!$.isFunction(n)) {
                    r = s;
                    s = n;
                    n = null
                }
                if (/^http/i.test(s)) {
                    r = s;
                    s = ""
                }
                if ($.isArray(e)) {
                    var o = e.length,
                    a = function(c) {
                        if (c < o) i("script", m(e[c], r), t,
                        function() {
                            a(c + 1)
                        },
                        s);
                        else $.isFunction(n) && n()
                    };
                    a(0)
                } else i("script", m(e, r), t, n, s);
                return this
            },
            loadCss: function(e, t) {
                if ($.isArray(e)) {
                    var n = e.length,
                    s = 0;
                    for (; s < n; s++) i("link", m(e[s], t))
                } else i("link", m(e, t));
                return this
            }
        })
    } ();
    function Class() {}
    !function(e) {
        if (e.Class !== Class) e.Class = Class;
        function i() {}
        Class.prototype.log = Class.prototype.warn = i;
        if (e.console) {
            Class.prototype.log = function() {
                console.log && console.log.apply(console, arguments)
            };
            Class.prototype.warn = function() {
                console.warn && console.warn.apply(console, arguments)
            }
        }
        var m = Class.prototype.warn;
        Class.prototype.callSuper = function() {
            m("父类没有同名方法，不能调用callSuper！")
        };
        Class.extend = function o(e, i) {
            var a, c, l = this.prototype;
            if (!i) {
                i = e;
                e = ""
            }
            if ("object" !== typeof i || !i.hasOwnProperty) {
                m("继承类的原型数据错误！");
                return
            }
            var u = n(e);
            if (!u) return;
            a = new this;
            for (var h in i) if (i.hasOwnProperty(h)) if ("function" == typeof i[h] && "function" == typeof l[h]) {
                var f = l[h];
                if (!f.__isAgent) f = s(r(h + "方法被子类覆盖，但是父类没有同名函数，不能调用callSuper!"), l[h]);
                a[h] = s(f, i[h])
            } else a[h] = i[h];
            function W() {}
            W.prototype = a;
            W.prototype.constructor = W;
            W.extend = o;
            W.create = t;
            u(W);
            return W
        };
        function t() {
            var e = new this;
            if (e.init) e.init.apply(e, arguments);
            return e
        }
        function n(e) {
            if (!e) return i;
            if (!/^(?:Base|Tools|Widgets|Game|Page)\./.test(e)) return m("Class命名空间错误，一级命名空间只能是:Base、Tools、Widgets、Game、Page");
            var t = e.split("."),
            n = t.length,
            s = 0,
            r = Class,
            o;
            for (; s < n - 1; s++) {
                o = t[s];
                r = r[o] = r[o] || {}
            }
            o = t[n - 1];
            if (r[o]) return m("已经有同名Class存在，请更换名称或路径！");
            return function(e) {
                r[o] = e
            }
        }
        function s(e, i) {
            var m = function() {
                var m = this.hasOwnProperty("callSuper"),
                t = this.callSuper,
                n;
                this.callSuper = e;
                n = i.apply(this, arguments);
                if (!m) delete this.callSuper;
                else this.callSuper = t;
                return n
            };
            m.__isAgent = true;
            return m
        }
        function r(e) {
            return function() {
                m(e)
            }
        }
    } (window);
    !function(e) {
        var i = Array.prototype.slice,
        m = Object.prototype.toString,
        t = function() {},
        n = 1,
        s = function(e) {
            return "[object Function]" == m.call(e)
        };
        var r = e.extend({
            init: function() {
                this.eventCache = this.eventCache || {}
            },
            createEvent: function(e, m) {
                if ("string" !== typeof e) return;
                var t = this,
                n = t.eventCache;
                $.each(e.split(" "),
                function(e, r) {
                    n[r] = n[r] || [];
                    m && (t[r] = function(e) {
                        if (s(e)) {
                            t.bind(r, e);
                            return this
                        } else return t.trigger.apply(t, [r].concat(i.call(arguments, 0)))
                    })
                })
            },
            trigger: function(e, m) {
                var t, n = 0,
                s = this,
                r = i.call(arguments, 1);
                if (!isNaN(e) && e && +e > 0) {
                    if ("string" !== typeof m) return 1;
                    t = this.eventCache[m || ""];
                    if (!t) return 2;
                    if (!t.length) return 0;
                    t.paras = r;
                    if (!t.t) t.t = window.setTimeout(function() {
                        delete t.t;
                        s.trigger.apply(s, t.paras)
                    }, parseInt(e, 10) || 200);
                    return 0
                }
                if ("number" === typeof e && (isNaN(e) || e < 0)) {
                    if ("string" !== typeof m) return 1;
                    t = this.eventCache[m || ""];
                    if (t) this.warn("事件" + m + "设置的缓冲保护时间不是合法数字")
                } else {
                    if ("string" !== typeof(e || m)) return 1;
                    t = this.eventCache[e || m || ""]
                }
                if (!t) return 2;
                $.each(t.slice(0),
                function(e, i) {
                    try {
                        if (false === i.apply(s, r)) n++
                    } catch(m) {
                        s.log(m);
                        return
                    }
                });
                return n ? false: 0
            },
            bind: function(e, i) {
                if ("string" !== typeof e) return 1;
                var m = this.eventCache[e];
                if (!m) return 2;
                if (!s(i)) return 3;
                i.muid = i.muid || n++;
                m.push(i);
                return 0
            },
            unbind: function(e, i) {
                if (0 === arguments.length) {
                    this.eventCache = {};
                    return 0
                }
                if ("string" !== typeof e) return 1;
                var m = this.eventCache[e || ""];
                if (!m) return 2;
                if (i === undefined) {
                    m.length = 0;
                    return this
                }
                if (!s(i)) return 3;
                for (var t = 0; t < m.length; t++) if (m[t] === i || i.muid && m[t].muid === i.muid) {
                    m.splice(t, 1);
                    t--
                }
                return 0
            },
            bindOnce: function(e, i) {
                if ("string" !== typeof e) return 1;
                var m = this.eventCache[e],
                t = this;
                if (!m) return 2;
                if (!s(i)) return 3;
                var r = function() {
                    var m = i.apply(this, arguments);
                    t.unbind(e, r);
                    return m
                };
                r.muid = i.muid = i.muid || n++;
                return t.bind(e, r)
            }
        });
        e.extend("Base.Message", {
            init: function() {
                this.__agent = this.__agent || r.create()
            },
            bindMsg: function(e, i, m, t) {
                if (!e || !s(i)) return this;
                this.__agent.createEvent(e);
                var n = m ?
                function() {
                    return i.apply(m, arguments)
                }: function() {
                    return i.apply(window, arguments)
                };
                n.muid = i.muid;
                this.__agent[t ? "bindOnce": "bind"](e, n);
                i.muid = n.muid;
                return this
            },
            bindMsgOnce: function(e, i, m) {
                return this.bindMsg(e, i, m, 1)
            },
            unbindMsg: function(e) {
                if (!e) return this;
                this.__agent.unbind.apply(this.__agent, arguments);
                return this
            },
            sendMsg: function(e) {
                this.__agent.trigger.apply(this.__agent, arguments);
                return this
            }
        }); !
        function(i, m) {
            var t = e.Base.Message.create();
            i.each(["bindMsg", "bindMsgOnce", "unbindMsg", "sendMsg"],
            function(e, n) {
                m[n] = i[n] = function() {
                    t[n].apply(t, arguments);
                    return this
                }
            })
        } (window.jQuery || window.Zepto, window.Zepto || window);
        r.extend("Base.Event", {
            init: function(e) {
                this.callSuper();
                this.createEvent(e, true);
                this.createEvent = t
            },
            trigger: function(e) {
                var i = this.callSuper.apply(this, arguments);
                if (i && !isNaN(i)) this.warn(["trigger事件名称必须是字符串", "未注册的事件(" + e + ")不能trigger"][i - 1]);
                if (false === i) return false
            },
            bind: function(e) {
                var i = this.callSuper.apply(this, arguments);
                if (i) this.warn(["bind事件名称必须是字符串", "未注册的事件(" + e + ")不能bind", "bind(" + e + ")注册事件必须是函数"][i - 1]);
                return this
            },
            unbind: function(e) {
                if (!e) {
                    this.warn("暂不支持全部事件一次性卸载");
                    return this
                }
                this.callSuper.apply(this, arguments);
                return this
            },
            bindOnce: function(e) {
                var i = this.callSuper.apply(this, arguments);
                if (i) this.warn(["bindOnce事件名称必须是字符串", "未注册的事件(" + e + ")不能bindOnce", "bindOnce(" + e + ")注册事件必须是函数"][i - 1]);
                return this
            }
        })
    } (window.Class);
} (window, jQuery);
var Core = function (e, t, n) {
    var a = {
        connectTime: e.performance && e.performance.timing ? e.performance.timing.connectStart || 0 : 0,
        serverInitTime: +new Date,
        localInitTime: +new Date,
        getServerTime: function () {
            var e = this.localInitTime - this.connectTime,
            i = this.serverInitTime + +new Date - this.localInitTime;
            return new Date(this.connectTime > 0 && e > 0 ? i + e : i)
        }
    },
    b = {
        appName: "彩咖网",
        home: window.location.protocol + "//" + window.location.host,
        regUrl: "member/register?username={username}&url=" + encodeURIComponent(e.document.URL) + "&loginurl=" + encodeURIComponent(e.document.URL),
        loginUrl: "member/login?username={username}&url=" + encodeURIComponent(e.document.URL),
        logoutUrl: "member/doLogout?username={username}&url=" + encodeURIComponent(e.document.URL),
        userUrl: "member/usercenter?username={username}",
        welcomeUser: "{time}好{nameHolder}，欢迎来到{appName}！{logoutLink}",
        welcomeGuest: "欢迎来到{appName}！{loginLink} {regLink}",
        logoutTxt: "安全退出",
        loginTxt: "请登录",
        regTxt: "免费注册",
        funcEntry: true,
    };
    var easyNav = {
        o : e.console ? function (e) {
            console.log(e)
        } : function () { },
        init: function (i) {
            easyNav.setWrap(i);
            easyNav.setConf()
        },
        reg: function () {
            this.addIframe();
            this.initIframe(this.getEntryUrl("regUrl"));
        },
        login: function () {
            this.addIframe(408);
            this.initIframe(this.getEntryUrl("loginUrl"));
        },
        getDiscoverCss: function () {
            var e = "";
            if (this.__coverBackground && this._$supportCss3("animation")) e = -1 != this.__coverBackground.indexOf("background:") ? this.__coverBackground : "";
            return "position:fixed;_position:absolute;top:0;left:0;width:100%;height:100%;overflow:hidden;background:rgb(0,0,0); filter:progid:DXImageTransform.Microsoft.Alpha(opacity=30);-moz-opacity:0.3;-khtml-opacity:0.3;opacity:0.3;z-index:1000;" + e
        },
        getPanelCss: function (e, t) {
            return "position:fixed;z-index:1001;left:50%;top:50%;width:" + e + "px;margin-left:-" + e / 2 + "px;height:" + t + "px;margin-top:-" + t / 2 + "px;"
        },
        getIframeCss: function () {
            var e = null;
            if (this.__iframeShowAnimation) var e = "-webkit-animation:" + this.__iframeShowAnimation + ";-moz-animation:" + this.__iframeShowAnimation + ";-ms-animation:" + this.__iframeShowAnimation + ";-o-animation:" + this.__iframeShowAnimation + ";animation:" + this.__iframeShowAnimation + ";";
            return "width:100%;height:100%;border:none;background:none;" + (e ? e : "")
        },
        addIframe: function (h, w) {
            var d = document.getElementById("x-URS");
            if (!d) {
                d = document.createElement("div");
                d.id = "x-URS";
                document.body.appendChild(d);
            }
            var a = w || 420, s = h || 500;
            var c = document.getElementById("x-discover");
            if (!c) {
                c = document.createElement("div");
                c.id = "x-discover";
                c.style.cssText = this.getDiscoverCss()
            }
            var l = document.getElementById("x-panel");
            if (!l) {
                l = document.createElement("div");
                l.id = "x-panel";
                l.style.cssText = this.getPanelCss(a, s)
            }
            var i = document.getElementById("x-URS-iframe");
            if (!i) {
                try {
                    i = document.createElement("<iframe allowTransparency=true ></iframe>")
                } catch (r) {
                    i = document.createElement("iframe");
                    i.allowTransparency = !0;
                }
                i.frameBorder = 0;
                i.id = "x-URS-iframe";
                i.scrolling = "no";
                i.style.cssText = this.getIframeCss()
            }
            l.appendChild(i);
            d.appendChild(c);
            d.appendChild(l);
        },
        initIframe: function (i) {
            var e = document.getElementById("x-URS-iframe");
            window.setTimeout(function () {
                e.src = i + "?v=" + +new Date;
            }, 0)
        },
        closeIframe: function (i) {
            var d = document.getElementById("x-URS");
            if (d) {
                $(d).html('');
            }
        },
        onLogin: function (e) {
            if (t.isFunction(e)) t.bindMsg("login.success", e);
            this.repaint();
        },
        setWrap: function (e) {
            this.wrap = this.wrap || t(e || "#topNavLeft")
        },
        setConf: function () {
            this.checkOptions();
            this.repaint()
        },
        checkOptions: function (e) {
            var m = this.options || t.extend({}, b);
            t.each(m, function (e, tc) {
                if ("string" === typeof t) m[e] = t.format(tc, m)
            });
            this.options = m
        },
        checkData: function () {
            var e = this.options;
            this.data = {
                appName:b.appName,
                regLink: easyNav.getUrl("javascript:Core.easyNav.reg();", e.regTxt),
                loginLink: easyNav.getUrl("javascript:Core.easyNav.login();", e.loginTxt),
                logoutLink: easyNav.getUrl(this.getEntryUrl("logoutUrl"), e.logoutTxt),
                time: easyNav.getTimeDesc(),
                nickName: (easyNav.lData && easyNav.lData.nick) || '游客',
                nameHolder: (easyNav.lData && easyNav.lData.account) || ''
            }
            if (this.lData && this.lData.flag == 'y') {
                $.extend(this.data, {
                    nickName: t.safeHTML(easyNav.lData.nick),
                    nameHolder: function () {
                        return easyNav.getEntry();
                    }()
                })
            }
        },
        repaint: function () {
            if (this.wrap) {
                this.wrap.empty().html(easyNav.getHTML());
            }
            if (this.lData && this.lData.flag == 'y') {
                t('#topNavRight>li.f-dn').removeClass('f-dn');
                t('#available').html(this.lData.available);
                t('#topInfo>a.topNavHolder').each(function () {
                    t(this).click(function () {
                        Core.userAct(t(this).attr('id'))
                    });
                });
            }
        },
        bindDropMenu: function (m, tt, n, s, r, o, a, c) {
            var l = r || t.noop,
            u, h = {
                mouseout: function (n) {
                    var r = n.relatedTarget || n.toElement;
                    if (r !== this && !t.contains(this, r) && r !== tt && !t.contains(t, r)) {
                        if (!f) tt.style.display = "none";
                        t(m).removeClass(s);
                        c && c()
                    }
                    u && e.clearTimeout(u)
                }
            },
            f = t.contains(m, tt);
            h[n || "click"] = function (m) {
                if (o && (n || "").indexOf("mouse") >= 0) {
                    var r = this;
                    u && e.clearTimeout(u);
                    u = e.setTimeout(function () {
                        u = 0;
                        if (!f) tt.style.display = "block";
                        t(r).addClass(s);
                        a && a(m)
                    }, o)
                } else {
                    if (!f) tt.style.display = "block";
                    t(this).addClass(s);
                    a && a(m)
                }
                t.contains(tt, m.target) || m.preventDefault()
            };
            t(m).bind(h);
            t(tt).bind({
                mouseout: function (tc) {
                    var n = tc.relatedTarget;
                    if (n !== this && !t.contains(this, n) && n !== m && !t.contains(m, n)) {
                        if (!f) this.style.display = "none";
                        t(m).removeClass(s);
                        c && c(tc)
                    }
                    u && e.clearTimeout(u)
                },
                click: function (e) {
                    var n = "a" == e.target.tagName.toLowerCase() ? e.target : "a" == e.target.parentNode.tagName.toLowerCase() ? e.target.parentNode : null;
                    if (!n || false !== l.call(n, e)) {
                        if (!f) t.style.display = "none";
                        t(m).removeClass(s);
                        c && c(e)
                    }
                }
            })
        },
        getHTML: function () {
            this.checkData();
            return t.format(this.options[(this.lData && this.lData.flag == 'y') ? "welcomeUser" : "welcomeGuest"], this.data);
        },
        getUrl: function (e, tc) {
            var n = /^javascript:/i.test(e) ? 'javascript:void(0);" onclick="' + e.substring(11) : e;
            n = t.format(n, {
                username: (easyNav.lData && easyNav.lData.account) || ''
            });
            return '<a href="' + n + '">' + tc + "</a>"
        },
        getTimeDesc: function () {
            var e = (new Date).getHours();
            return e > 5 && e <= 11 ? "上午" : e > 11 && e <= 13 ? "中午" : e > 13 && e <= 17 ? "下午" : e > 17 || e <= 2 ? "晚上" : "凌晨"
        },
        getEntry: function () {
            var i = [
                '<span id="userBox">',
                '<a href="javascript:void(0);" onclick="Core.userAct()" id="userName" title="{username}"><em>[ {username} ]</em></a></span>'
            ].join("");
            var a = {
                ursId: t.safeHTML(this.lData.account),
                username: t.safeHTML(this.lData.nick),
            };
            return t.format(i, a)
        },
        getEntryUrl: function (e) {
            var a = {
                username: (easyNav.lData && easyNav.lData.flag == 'y') ? t.safeHTML(easyNav.lData.account) : ''
            };
            return c.cdnUrl + t.format(Core.easyNav.options[e], a)
        },
        tools: {
            getUrlDomain: function (e) {
                var i = (e || n.URL).replace(/\?.*$/g, "").replace(/#.*$/g, "");
                if (/^[^:]+:\/\/([^\/\?\#]+).*$/gi.test(i)) return RegExp.$1;
                return i
            },
            checkUrlDomain: function (e, i) {
                return new RegExp(i + "$", "i").test(e)
            }
        }
    };
    var c = {
        cdnUrl: "",
        version: "2.01",
        serverTime: function () {
            return a.getServerTime()
        },
        now: function () {
            return (new Date).getTime()
        },
        GC: e.CollectGarbage || t.noop,
        log: t.getPara("debugger") && e.console ? function () {
            return e.console.log.applay(e.console, arguments)
        } : t.noop,
        bindModule: function (e) {
            t.bindModule(this, e, this.cdnUrl);
            return this
        },
        dealCPPara: function () {
            var i = e.document,
            m = i.getElementsByTagName("a"),
            t = m.length,
            n = 0,
            s,
            r,
            o;
            for (; n < t; n++) {
                r = m[n];
                s = r.getAttribute("cppara") || "";
                if (s) {
                    o = r.innerHTML;
                    r.setAttribute("href", r.getAttribute("href") + "?" + s);
                    r.removeAttribute("cppara");
                    r.innerHTML = o
                }
            }
            return this
        },
        loadCdnJS: function () {
            var e = arguments;
            Array.prototype.push.call(arguments, this.cdnUrl);
            return t.loadJS.apply(this, e)
        },
        loadCdnCss: function (e) {
            return t.loadCss(e, this.cdnUrl)
        },
        configInit: function (n, o, r, s) {
            this.cdnUrl = n;
            this.version = o;
            if (s) a.serverInitTime = +s;
            t.bindModule({
                dialog: {
                    js: "content/js/com/dialog.js"
                },
                scrollWhenNeed: {
                    js: "content/js/easyTools/scroll.js"
                },
                easyEvents: {
                    js: "content/js/easyTools/event.js"
                }
            },
            this.cdnUrl);
            t.bindModule(t.fn, {
                "disableSelection enableSelection disableRightClick enableRightClick disableIME enableIME setControlEffect": {
                    js: "content/js/easyTools/tools.js"
                },
                bindDrag: {
                    js: "content/js/easyTools/drag.js"
                },
                "scrollGrid xScrollGrid": {
                    js: "content/js/easyTools/scroll.js"
                },
                easyEvents: {
                    js: "content/js/easyTools/event.js"
                },
                placeholder: {
                    js: "content/js/placeholder.js"
                },
                carousel: {
                    js: "content/js/com/carousel.js"
                }
            },
            this.cdnUrl);
            this.bindModule({
                "shareAction shareGet sharePost": {
                    js: "content/js/shareAction.js"
                }
            });

            this.loadGolbalConfig && this.loadGolbalConfig();
            delete this.configInit
        },
        loadGolbalConfig: function () {
            e.gameActConf ? jQuery.sendMsg("lotteryList") : this.loadCdnJS("content/static/lotteryList.html?" + +new Date,
                function () {
                    jQuery.sendMsg("lotteryList")
            })
        },
        navInit: function (e, n, i, t) {
            this.configInit && this.configInit(e, n, i, t);
            easyNav.init();
            delete this.navInit
        },
        fastInit: function (e) {
            easyNav.onLogin(function (d) {
                easyNav.repaint();
            });
            if (c.eventWrap && c.events && t.fn.easyEvents) t(c.eventWrap).easyEvents(c.events, c);
            c.navInit && c.navInit(c.cdnUrl, +new Date, "", true);
            c.isLogin(function (e) {
                if (e && e.flag == 'y') {
                    t.sendMsg('login.success', e);
                    c.loadCdnJS("content/js/userCenter/userMessage.js");
                }
            });
            try {
                c.initLotteryList();
                c.dealCPPara();
            } catch (m) { }

            delete c.fastInit;
            e && c.quickInit && c.quickInit()
        },
        initLotteryList: function () {
            var n = t("#lotteryListEntry"),
            i = t("#lotteryList"),
            o = t("#funcTab"),
            r = t(".lotteryListWrap", i),
            a = false,
            s,
            c = function (e) {
                e.prepend('<iframe class="iFrameGround" frameborder="0"></iframe>');
                e.find(".iFrameGround:first").width(e.outerWidth()).height(e.outerHeight())
            },
            l = function (e) {
                if (!t.isIE678) {
                    i.show(); !a && r.stop().animate({
                        height: s
                    },
                    300,
                    function () {
                        r.css("overflow", "visible");
                        a = true
                    });
                    a = true
                }
            },
            u = function (e) {
                if (!t.isIE678) {
                    if (e && "click" === e.type) {
                        n.addClass("open");
                        return
                    }
                    a && r.stop().animate({
                        height: 0
                    },
                    300,
                    function () {
                        r.css("overflow", "visible");
                        i.hide();
                        a = false
                    });
                    a = false
                }
            },
            f = function () {
                var e = t(".otherGames", i),
                n = 100;
                i.css("display", "block");
                e.each(function (e, i) {
                    var o = t(this),
                    r = t("h3", o),
                    a = t(".exBox", o),
                    s,
                    c,
                    l = +o.attr("data-maxrow"),
                    u,
                    f = o.height();
                    r.css({
                        height: f - (f / 2 - 16),
                        "padding-top": f / 2 - 16
                    });
                    s = t.makeArray(a.children());
                    if (a[0]) {
                        c = s.length;
                        u = Math.ceil(c / l);
                        a.height(f);
                        a.width(u * n);
                        a.empty();
                        for (var e = 0; e < u; e++) {
                            var d = t("<div></div>");
                            d.css({
                                width: n,
                                height: f,
                                "float": "left"
                            });
                            d.append(s.slice(e * l, (e + 1) * l));
                            a.append(d)
                        }
                        o.hover(function () {
                            o.addClass("otherGamesOn");
                            o.prev().css("border", "0")
                        },
                        function () {
                            o.prev().removeAttr("style");
                            o.removeClass("otherGamesOn")
                        })
                    }
                })
            };
            r.delegate("a[href]", "click", function (e) {
                var n;
                n = this.getAttribute("href", 2);
                n = /^http/.test(n) ? n : location.protocol + "//" + location.host + n;
                if ("_blank" !== t(this).attr("target") && n.split("#")[0] == location.href.split("#")[0]) {
                    location.href = n;
                    location.reload();
                    e.preventDefault()
                }
            });
            if (i[0]) {
                function d() {
                    var n = e.lotteryListConf,
                    i, o, a, s, c, l, u, d = "",
                    p = ["", "高频", "竞技"],
                    g = ["", 4, 2],
                    h = {
                        jclq_mix_p: "jclq"
                    },
                    m;
                    if (n) {
                        i = n.top;
                        o = n.gp;
                        a = n.jj;
                        m = t.map([i, o, a], function (e, n) {
                            if (0 === n) {
                                c = '<li class="zyGame {className}"><a gid="{gameEn}" href="{url}#from=leftnav"><em class="cz_logo35 logo35_{logoName}"></em><strong>{gameCn}</strong>{grayHTML}{redHTML}</a></li>';
                                l = "";
                                u = ""
                            } else {
                                c = '<em class="{className}"><a gid="{gameEn}" title="{title}" href="{url}#from=leftnav">{gameCn}{redHTML}</a></em>';
                                l = '<li class="otherGames clearfix ' + (2 === n ? "end" : "") + '" data-maxrow=' + g[n] + "><h3>" + p[n] + "</h3><div>";
                                u = "</div></li>";
                                d = '<i class="exArrow">&gt;</i></div><div class="exBox">'
                            }
                            return l + t.map(e, function (e, i) {
                                var o = "";
                                if (0 === n) if (e.isTJ) o = "tjGame";
                                return (i == 2 * g[n] ? d : "") + t.format(c, t.extend({}, e, {
                                    url: location.protocol + "//" + location.host + "/" + e.url.replace(/\#from\=.*/, ""),
                                    className: o,
                                    grayHTML: e.gray ? '<span class="grayWords">' + e.gray + "</span>" : "",
                                    redHTML: e.red ? '<span class="redWords"><i class="arrowsIcon"></i>' + e.red + "</span>" : "",
                                    logoName: h[e.gameEn] || e.gameEn
                                }))
                            }).join("") + u
                        }).join("");
                        r.find("ul").html(m);
                        f();
                        r.trigger("contentChange")
                    }
                }
                if (e.lotteryListConf) d();
                else t.bindMsgOnce("lotteryList", d);
                this.gameAct.list(function (e, n) {
                    var i = "cz_" + n;
                    o.find("[pid=" + e + "]").each(function () {
                        t(this).find("a").prepend("<span class='" + i + "'></span>")
                    })
                })
            }
            if (n[0] && i[0] && t.contains(n[0], i[0])) {
                this.easyNav.bindDropMenu(n[0], i[0], "mouseover", "open", t.noop, 200, l, u);
                if (t.isIE678) c(i);
                else {
                    s = r.outerHeight();
                    i.hide().css({
                        left: 0
                    });
                    r.height(0);
                    r.bind("contentChange", function () {
                        if (!r.is(":animated")) {
                            if (!a) i.css({
                                left: -9999
                            }).show();
                            r.height("auto");
                            s = r.outerHeight();
                            if (!a) {
                                i.hide().css({
                                    left: 0
                                });
                                r.height(0)
                            }
                        }
                    })
                }
            }
            delete this.initLotteryList;
            return this
        },
        init: function () {
            this.fastInit && this.fastInit(1);
            t("#showmore, .showmore").click(function () {
                var e = i(this).text();
                if (/更多/.test(e)) {
                    i(this).text(e.replace(/更多/, "收起"));
                    i(this).parent().css("height", "auto")
                } else {
                    i(this).text(e.replace(/收起/, "更多"));
                    i(this).parent().removeAttr("style")
                }
            });
            try {
                this.myInit()
            } catch (s) { }
            t(".mcDropMenuBox").each(function () {
                easyNav.bindDropMenu(this, t(".mcDropMenu", this)[0], "mouseover", "dropMenuBoxActive", e, 200);
                t(".topNavHolder", this).click(e)
            });
            t(".wordsNum2,.wordsNum4", "#funcTab").each(function () {
                easyNav.bindDropMenu(this, t(".mcDropMenu", this)[0], "mouseover", "hover", t.noop, 200)
            });
            delete this.init;
            delete this.myInit;
            this.quickInit && delete this.quickInit;
            this.GC()
        },
        emptySendHttp: function (i) {
            var m = "imgLoad_" + +new Date + parseInt(100 * Math.random()),
            t,
            n;
            t = e[m] = new Image;
            t.onload = function () {
                e[m] = null;
                _ntes_void()
            };
            t.onerror = function () {
                e[m] = null
            };
            i = i.replace(/#\S*$/, "");
            n = (i + "").indexOf("?") + 1 ? "&" : "?";
            t.src = i + n + "d=" + +new Date;
            t = null
        },
        myInit: t.noop,
        get: t.get2,
        post: t.post2,
        getJSON: t.getJSON2,
        postJSON: t.postJSON2,
        parseJSON: function (e) {
            e = e.replace(/("|')\\?\/Date\((-?[0-9+]+)\)\\?\/\1/g, "new Date($2)");
            return new Function("return " + e)()
        },
        isLogin: function (e) {
            this.postJSON(this.cdnUrl + "member/queryLoginStatus", function (t, n) {
                easyNav.lData = n;
                if (t === 0) {
                    e && e.call(this, easyNav.lData)
                }
            })
        },
        userAct: function (m) {
            this.isLogin(function (e) {
                if (e && e.flag != 'y') {
                    easyNav.login();
                }
                else {
                    if (m == undefined || m == '')
                        window.location = this.cdnUrl + 'member/usercenter/';
                    else
                        window.location = this.cdnUrl + 'member/usercenter/' + m;
                }
            })
        },
        gameAct: {
            is: function (m, t, n) {
                if (!c.gameActConfig) {
                    c.loadCdnJS("content/js/config.js", function () {
                        c.gameAct.is(m, t, n)
                    });
                    return
                }
                var s = c.gameActConfig[m],
                r = c.serverTime(),
                o = false,
                a,
                l = 0,
                u = "",
                h;
                if (jQuery.isFunction(t)) {
                    n = t;
                    t = ""
                }
                if (s) {
                    h = jQuery.isFunction(n) ?
                    function (e) {
                        n.call(s, e)
                    } : jQuery.noop;
                    a = jQuery.isArray(s.type);
                    if (s.range) {
                        if ("server" == s.range) {
                            var f = function () {
                                var i = e.gameActConf,
                                n = false;
                                if (!i) return;
                                if (i[m]) n = t ? a ? s.type[i[m] - 1] == t : s.type == t : a ? s.type[i[m] - 1] : s.type;
                                return n
                            };
                            if (e.gameActConf) o = f();
                            else {
                                jQuery.isFunction(h) && jQuery.bindMsgOnce("lotteryList", function () {
                                    h(f())
                                });
                                return
                            }
                            jQuery.isFunction(h) && h(o);
                            return o
                        }
                        jQuery.each(s.range, function (e, i) {
                            var m, t, n, c;
                            if (i.indexOf("-") + 1) {
                                c = i.split("-");
                                if (c[1].indexOf(":") < 0) c[1] += " 23:59:59";
                                m = new Date(c[0]);
                                t = new Date(c[1])
                            } else {
                                n = new Date(i);
                                m = new Date(n.getFullYear(), n.getMonth(), n.getDate());
                                t = new Date(n.getFullYear(), n.getMonth(), n.getDate() + 1)
                            }
                            if (r >= m && r <= t) {
                                o = true;
                                l = e;
                                if (a && e >= s.type.length) l = 0;
                                return false
                            }
                        });
                        u = a ? s.type[l] : s.type
                    } else {
                        o = r <= s.end && (s.start ? r >= s.start : true);
                        u = a ? s.type[0] : s.type
                    }
                    o = t ? o && u == t : o ? u : ""
                }
                jQuery.isFunction(h) && h(o)
            },
            list: function (e) {
                if (!jQuery.isFunction(e)) return;
                var m = this,
                t = function (i) {
                    m.is(i, function (m) {
                        m && e.call(this, i, m)
                    })
                };
                if (!c.gameActConfig) c.loadCdnJS("content/js/config.js", function () {
                    for (var e in c.gameActConfig) t(e)
                });
                else for (var n in c.gameActConfig) t(n)
            }
        },
        css3: {
            test: function (e, t) {
                var i = document.createElement("div").style,
                o = ["Webkit", "Moz", "O", "ms"],
                r = e.charAt(0).toUpperCase() + e.slice(1),
                a = (e + " " + o.join(r + " ") + r).split(" ");
                for (var s in a) if (i[a[s]] !== n) return t ? a[s] : true;
                return t ? "" : false
            },
            getName: function (e) {
                var t = "",
                n = {
                    WebkitAnimation: "webkitAnimationEnd",
                    OAnimation: "oAnimationEnd",
                    msAnimation: "MSAnimationEnd",
                    animation: "animationend"
                };
                switch (e.toLowerCase()) {
                    case "animationend":
                        t = n[this.test("animation", true)];
                        break;
                    default:
                        t = this.test(e, true)
                }
                return t
            }
        }
    };
    c.easyNav = easyNav;
    return c;
}(window, jQuery);
jQuery(window).unload(function () {
    document.oncontextmenu = null;
    window.Core = null;
    window.onload = null;
    window.onresize = null;
    window.onunload = null;
    window.onerror = null;
    window.onbeforeunload = null; (window.CollectGarbage || function () {
    })()
});
jQuery(document).ready(function () {
    Core.init && Core.init()
});