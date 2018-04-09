!function (t, core, e, a) {
    var __page;
    var p = window.parent, c = p.Core;
    var verifyForm = function (m) {
        var b = true, r='',
        t = e(m).find('input.j-inputtext[data-required=true]');
        if (t.attr('value') == '') {
            b = false;
            r = '输入项不能为空。';
        } else {
            switch (t.attr('id')) {
                case "account":
                    if (!/^[a-zA-Z0-9\-_@.]{1,16}$/.test(t.attr('value'))) {
                        b = false;
                        r = '账号格式错误；只能是字母，数字和下划线。';
                    }
                    break;
                case "password":
                    if (t.attr('value').length < 6) {
                        b = false;
                        r = '密码长度不能少于6位。';
                    }
                    break;
                case "repassword":
                    var m = e('#password');
                    if (m) {
                        if (t.attr('value') !== m.attr('value')) {
                            b = false;
                            r = '两次输入的密码不一致。';
                        }
                    }
                    break;
                case "code":
                    if (!/^[a-zA-Z0-9]{4,4}$/.test(t.attr('value'))) {
                        b = false;
                        r = '验证码格式错误；只能是字母，数字且长度为4位。';
                    }
                    break;
            }
        }
        e(m).removeClass('error-color')
        e('#nerror').addClass('f-dn');
        e('#nerror>div.ferrorhead').html('');
        if (b !== true) {
            e(m).addClass('error-color')
            e('#nerror').removeClass('f-dn');
            e('#nerror>div.ferrorhead').html(r);
        }

        return b;
    };
    var verifyCode = function (a) {
        core.getJSON(c.cdnUrl + 'member/getvCode', {}, function (m, i, s) {
            var b = true;
            if (s == 'success' && i != '') {
                if (i.toLowerCase() != e('#code').attr('value').toLowerCase()) {
                    b = false;
                    r = '验证码输入错误。';
                }
            }
            else {
                b = false;
                r = '验证码已经失效。';
            }
            if (b !== true) {
                e('#code.inputbox').addClass('error-color')
                e('#nerror').removeClass('f-dn');
                e('#nerror>div.ferrorhead').html(r);
                e('#valiCode').attr('src', c.cdnUrl + "member/getvImage?v=" + +new Date());
            } else {
                a.call();
            }
        })
    };
    core.loadGolbalConfig = e.loop;
    e.extend(core, {
        setPage: function (d) {
            __page = d;
        },
        quickInit: function () {
            if (__page === "login") {
                e('#cnt-box-parent.g-bd').attr('style', 'height:402px');
            }
            e('.inputbox').each(function (i) {
                var m = e(this), c = m.find('div.u-tip');
                m.bind({
                    click: function () {
                        e('.inputbox').removeClass('active');
                        e(this).addClass('active');
                    },
                    change: function () {
                        verifyForm(m);
                    }
                });
                m.on('input', function () {
                    if (e(m.find('input')).attr('value').length > 0)
                        c.attr('style', 'display:block;');
                });
                c.bind({
                    click: function () {
                        m.find('input.j-inputtext').attr("value", "");
                        c.attr('style', 'display:none;');
                    }
                });
            });
            e('#un-login[type=checkbox]').bind({
                click: function () {
                    var c = e(this.parentNode);
                    if (c) {
                        e(c).hasClass('u-checkbox-select') ? c.removeClass('u-checkbox-select') : c.addClass('u-checkbox-select')
                    }
                }
            })
            e(".u-closebtn[data-action=doclose]").bind({
                click: function (m) {
                    if (__page == 'register') {
                        var t;
                        e('input.j-inputtext').each(function () {
                            var v = e(this).attr('value');
                            t = v || t;
                            if (t) {
                                return;
                            }
                        });
                        if (t) {
                            e("#confirm").removeClass("f-dn");
                            e("#cnt-box-parent").addClass("f-dn");
                        }
                        else {
                            c.easyNav.closeIframe();
                        }
                    }
                    else {
                        c.easyNav.closeIframe();
                    }
                }
            })
            e(".u-btn-confirm[data-action=confirmclose]").bind({
                click: function (m) {
                    c.easyNav.closeIframe();
                }
            })
            e(".u-btn-confirm[data-action=confirmgoon]").bind({
                click: function (m) {
                    e("#confirm").addClass("f-dn");
                    e("#cnt-box-parent").removeClass("f-dn");
                }
            })
            e('.u-loginbtn').bind({
                click: function () {
                    var b = true;
                    e('.inputbox').each(function () {
                        if (verifyForm(this) != true) {
                            b = false;
                            return false;
                        }
                    });
                    if (b === true) {
                        verifyCode(function () {
                            m = {
                                account: e('#account').attr('value'),
                                password: e('#password').attr('value'),
                                vcode: e('#code').attr('value'),
                            };
                            var url = '', m;
                            switch (__page) {
                                case "register":
                                    url =  c.cdnUrl + 'member/doregister';
                                    break;
                                case "login":
                                    e.extend(m, { remeber: e('#un-login').attr('value') });
                                    url = c.cdnUrl + 'member/dologin';
                                    break;
                            }
                            e('.u-loginbtn').addClass('btndisabled');
                            core.postJSON(url, m, function (a, b, m) {
                                if (b.state == 'success') {
                                    c.easyNav.closeIframe();
                                    p.window.location.reload();
                                }
                                else {
                                    e('#nerror').removeClass('f-dn');
                                    e('#nerror>div.ferrorhead').html(b.message);
                                    e('#valiCode').attr('src', c.cdnUrl + "member/getvImage?v=" + +new Date());
                                    e('.u-loginbtn').removeClass('btndisabled');
                                }
                            })
                        })
                    }
                }
            });
            e('#valiCode').bind({
                click: function () {
                    this.src = c.cdnUrl + "member/getvImage?v=" + +new Date()
                }
            });
            e('input').bind({
                keydown: function (e) {
                    var e = e || event, keycode = e.which || e.keyCode;
                    if (keycode == 13) {
                        jQuery('.u-loginbtn').trigger("click");
                    }
                }
            });

            e('div.u-tip').attr('style', 'display:none;');
        }
    });
}(window, Core, jQuery);