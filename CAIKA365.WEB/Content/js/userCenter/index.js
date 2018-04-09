!function (w, core, t, a) {
    t.extend(core, {
        quickInit: function () {
            t('.filter').delegate('div.user-sel', {
                'mouseover': function (e) {
                    t(this).addClass('hover');
                },
                'mouseleave': function (e) {
                    t(this).removeClass('hover');
                }
            });
            t('.sel-menu').delegate('li', 'click', function (e) {
                var a = t(this).parents("div.user-sel").find('span.sel-hd>em'),
                    b = t(this).parents("div.user-sel").find('input[type=hidden].sel-val');
                a.html(t(this).html());
                b.attr('value', t(this).attr('val'));
                t(this).parents("div.user-sel").removeClass('hover');
                var fn = t(this).attr('act');
                if (fn != undefined)
                    core[fn]({ t: $('#pro_type').val(), m: $('#pro_time').val() });
            })
            t('.pageup').on('click', function (e) {
                var p = parseInt((t('.go_page_no').attr('value') || 1)) - 1;
                if (p < 1)
                    p = 1;
                t('.go_page_no').attr('value', p);
                var fn = t(this).attr('act');
                if (fn != undefined) {
                    switch (fn) {
                        case "loadJournal":
                            core[fn]({ t: $('#pro_type').val(), m: $('#pro_time').val(), pageno: $('.go_page_no').val() });
                            break;
                        case "loadMsglist":
                            core[fn]({ pageno: $('.go_page_no').val() });
                            break;
                    }
                }
            })
            t('.pagedown').on('click', function (e) {
                var p = parseInt(t('.go_page_no').attr('value')) || 1;
                p = p + 1;
                if (parseInt(t('.pagesub').attr('pagenum') || 0) < p) {
                    p = parseInt(t('.pagesub').attr('pagenum')) || 1;
                }
                t('.go_page_no').attr('value', p);
                var fn = t(this).attr('act');
                if (fn != undefined) {
                    switch (fn) {
                        case "loadJournal":
                            core[fn]({ t: $('#pro_type').val(), m: $('#pro_time').val(), pageno: $('.go_page_no').val() });
                            break;
                        case "loadMsglist":
                            core[fn]({ pageno: $('.go_page_no').val() });
                            break;
                    }
                }
            })
            t('.btn-middle').on('click', function (e) {
                var p = parseInt(t('.go_page_no').attr('value')) || 1;
                var fn = t(this).attr('act');
                if (fn != undefined) {
                    switch (fn) {
                        case "loadJournal":
                            core[fn]({ t: $('#pro_type').val(), m: $('#pro_time').val(), pageno: p });
                            break;
                        case "loadMsglist":
                            core[fn]({ pageno: $('.go_page_no').val() });
                            break;
                    }
                }
            })
            t('.bank-list').delegate('li', 'click', function (e) {
                t('.bank-list>li').each(function () {
                    t(this).removeClass('active');
                })
                t(this).addClass('active');
                t('#payid').attr('value', t(this).attr('val'));
            })
            t('.tit-tab').delegate('li', 'click', function (e) {
                t('#channel').attr('value', '');
                t('#payid').attr('value', '');
                t('.bank-list>li').each(function () {
                    t(this).removeClass('active');
                })
                var r = t(this).attr('rel');
                if (r != undefined) {
                    t('.tit-tab>li').each(function () {
                        var a = t(this).attr('rel');
                        if (a != undefined) {
                            t(a).addClass('f-dn');
                        }
                        t(this).removeClass('active');
                    })
                    t(r).removeClass('f-dn');
                    t(this).addClass('active');
                    t('#channel').attr('value', t(this).attr('val'));
                }
            })
            t('span.ubtn3').on('click', function (e) {
                var a = core, fn = t(this).attr('act');
                if (fn != undefined)
                    a[fn]();
            })
            t('#amount').bind({
                keypress: function (e) {
                    return (/[\d.]/.test(String.fromCharCode(event.keyCode)))
                },
                blur: function (e) {
                    if (parseFloat(t('#amount').attr('value')) < 1) {
                        t('#amount').attr('value', 1);
                    } else if (parseFloat(t('#amount').attr('value')) > 999999) {
                        t('#amount').attr('value', 999999);
                    }
                }
            })
            t('input[type=password]').bind({
                keydown: function (e) {
                    if (e.keyCode == 32) {
                        return false;
                    }
                },
                change: function (e) {
                    core.checkPwd(this)
                },
                blur: function (e) {
                    core.checkPwd(this)
                }
            })
            t('#msglist').delegate('a[act="deleteMsg"]', 'click', function () {
                var o = t(t(this).closest('tr')).attr('id'),
                    tr = t(this).closest('tr');
                t.dialog({
                    title: '系统提示',
                    content: '是否确认删除此信息？',
                    animate: 10,
                    width: 385,
                    height: 180,
                    callback: function (b) {
                        if (b != null) {
                            core.post(core.cdnUrl + 'member/deleteMessage', { msgid: o }, function () {
                                t(tr).remove();
                                var o = parseInt(t('span#record_count').text().trim());
                                if (o) {
                                    o = o - 1;
                                    if (o < 0)
                                        o = 0;
                                }
                                t('span#record_count').text(o);
                                var i = parseInt(t('#newCount').text().trim());
                                if (i) {
                                    i = i - 1;
                                    if (i < 0)
                                        i = 0;
                                }
                                t('#newCount').text(i);
                            })
                        }
                    }
                });
            })
            t('#msglist').delegate('a[act="readMsg"]', 'click', function () {
                var o = t(t(this).closest('tr')).attr('id'),
                    tr = t(this).closest('tr');
                core.get(core.cdnUrl + 'member/getMessageBody', { msgid: o }, function (e, r, d) {
                    if (d == "success") {
                        t.dialog({
                            title: '用户消息',
                            content: $.parseJSON(r).msgbody,
                            animate: 10,
                            width: 485,
                            height: 380,
                            callback: function (b) {
                                t(tr).css('font-weight', 'normal');
                                var i = parseInt(t('#newCount').text().trim());
                                if (i) {
                                    i = i - 1;
                                    if (i < 0)
                                        i = 0;
                                }
                                t('#newCount').text(i);
                            }
                        });
                    }
                })
            })
            t('.list').on('mouseleave', function (e) {
                t(this).parent('div.u-select').removeClass('u-select-show');
                core.checkUSelect(this);
            })
            t('span.txt-box').on('click', function (e) {
                t('div.u-select').each(function (e) {
                    t(this).removeClass('u-select-show');
                })
                t(this).parent('div.u-select').addClass('u-select-show');
            })
            t('input[required=true]').on('change blur', function (e) {
                core.checkForms(this);
            })
            t('.list,.back-list').delegate('li,span', 'click', function (e) {
                var p = t(this).closest('div.u-select');
                var s = p.find('span>em.txt'),
                    v = p.find('input[type=hidden]');

                t(s).html('');
                t(s).append(t(this).find('i').clone(true));
                t(s).append(t(this).text());
                v.val(t(this).attr('val'));
                p.removeClass('u-select-show');
                var rel = p.attr('rel') || '';
                if (rel != '') {
                    t(rel).find('input[type=hidden]').val('');
                    var ul = t(rel).find('ul.list').html('');
                    var txt = t(rel).find('em.txt').html('请选择');
                    core.get(core.cdnUrl + 'home/getAreas/' + v.val(), {}, function (e, r) {
                        var data = $.parseJSON(r);
                        if (data.flag == 'y') {
                            if (ul != undefined) {
                                var i = "";
                                for (var i = 0; i < data.rows.length; i++) {
                                    t(ul).append("<li val='" + data.rows[i].id + "'>" + data.rows[i].name1 + "</li>");
                                }
                            }
                        }
                    })
                }
            })
            t('a[t=request]').on('click', function (e) {
                core.postrequest(this);
            })
            this.loadProvinces && this.loadProvinces();
        },
        addDisCover: function () {
            var p = document.getElementsByClassName('order-bd')[0];
            var d = document.getElementById("x-URS");
            if (!d) {
                d = document.createElement("div");
                d.id = "x-URS";
                p.appendChild(d);
            }
            var c = document.getElementById("x-discover");
            if (!c) {
                c = document.createElement("div");
                c.id = "x-discover";
                c.style.cssText = "position:absolute;top:" + $(p).position().top + "px;left:" + $(p).position().left + ";width:820px;height:100%;overflow:hidden;background:rgb(0,0,0); filter:progid:DXImageTransform.Microsoft.Alpha(opacity=30);-moz-opacity:0.3;-khtml-opacity:0.3;opacity:0.3;z-index:1000;";
            }

            var l = document.getElementById("x-panel");
            if (!l) {
                l = document.createElement("div");
                l.id = "x-panel";
                l.innerHTML = '<div style="text-align:center;"><img style="margin-top:3px" src="../../css/imgs/loading.gif" /><span style="margin-left:10px;vertical-align:middle"><em>处理中……</em></span></div>';
                l.style.cssText = "position:fixed;z-index:1001;left:50%;top:50%;background-color:white;width:160px;height:40px;";
            }
            d.appendChild(c);
            d.appendChild(l);
            return d;
        },
        loadProvinces: function () {
            core.get(core.cdnUrl + 'home/getAreas/', {}, function (e, r) {
                var data = $.parseJSON(r);
                if (data.flag == 'y') {
                    var ul = t("ul#Provinces.list").html('');
                    var i = "";
                    for (var i = 0; i < data.rows.length; i++) {
                        t(ul).append("<li val='" + data.rows[i].id + "'>" + data.rows[i].name1 + "</li>");
                    }
                }
            })
        },
        loadJournal: function (d) {
            var c = core.addDisCover();
            core.isLogin(function (e) {
                if (e && e.flag != 'y') {
                    $(c).remove();
                    core.easyNav.login();
                }
                else {
                    if (d.pageno == null || d.pageno == 0) {
                        d.pageno = 1;
                    }
                    core.post(core.cdnUrl + 'member/queryJournalList', d, function (r, v, d) {
                        if (d == 'success') {
                            var data = $.parseJSON(v);
                            $('#deposited').html(data.deposited);
                            $('#bonus').html(data.bonus);
                            $('#exchanged').html(data.exchanged);
                            $('#returned').html(data.returned);
                            $('#consume').html(data.consume);
                            $('#withdraw').html(data.withdraw);
                            $('em.record_count').html("[" + data.rows.length + "]");
                            $('em.page_count').html("[" + data.pagenums + "]");
                            $('#pro_betlist').html('');
                            $('.go_page_no').val(data.pageno);
                            $('span.pagesub').attr('pagenum', data.pagenums);
                            if (data.flag == 'y') {
                                if (data.rows.length == 0) {
                                    $('#pro_betlist').html('<tr class="un"><td colspan="7">暂无记录</td></tr>');
                                }
                                else {
                                    for (var i = 0; i < data.rows.length; i++) {
                                        var m = data.rows[i];
                                        var cls = "";
                                        if (i % 2 == 1) {
                                            cls = "style=background:#f3f2f2;";
                                        }
                                        var type_title = "";
                                        switch (m.type) {
                                            case "cz":
                                                type_title = "充值";
                                                break;
                                            case "tx":
                                                type_title = "提现";
                                                break;
                                            case "tz":
                                                type_title = "投注";
                                                break;
                                            case "zj":
                                                type_title = "中奖";
                                                break;
                                            case "cd":
                                                type_title = "撤单";
                                                break;
                                            case "fd":
                                                type_title = "返点";
                                                break;
                                            case "jfdh":
                                                type_title = "积分兑换";
                                                break;
                                            case "hdps":
                                                type_title = "活动派送";
                                                break;
                                        }
                                        var amount = "";
                                        if (parseFloat(m.amount) > 0) {
                                            amount = "<td style='color: red; font-weight:bold'>+" + m.amount + "</td>"
                                        } else {
                                            amount = "<td style='color: green; font-weight:bold'>-" + m.amount + "</td>"
                                        }
                                        var state = "";
                                        switch (m.state) {
                                            case "Checked":
                                                state = "<td style='color:red'>已审核</td>";
                                                break;
                                            case "UnCheck":
                                                state = "<td style='color:green'>待审核</td>";
                                                break;
                                        }
                                        $('#pro_betlist').append("<tr " + cls + "><td>" + m.t_id + "</td><td>" + m.time + "</td><td>" + type_title + "</td>" + amount + "<td>" + m.available + "</td><td>" + m.remark + "</td>" + state + "</tr>");
                                    }
                                }
                            }
                        }
                        $(c).remove();
                    })
                }
            });
        },
        loadMsglist: function (d) {
            var c = core.addDisCover();
            core.isLogin(function (e) {
                if (e && e.flag != 'y') {
                    $(c).remove();
                    core.easyNav.login();
                }
                else {
                    if (d.pageno == null || d.pageno == 0) {
                        d.pageno = 1;
                    }
                    core.post(core.cdnUrl + 'member/queryMessageList', d, function (r, v, d) {
                        if (d == 'success') {
                            var data = $.parseJSON(v);
                            if (data.flag == 'y') {
                                $('#msglist').html('');
                                $('.go_page_no').val(data.pageno);
                                $('span.pagesub').attr('pagenum', data.pagenums);
                                if (data.rows.length == 0) {
                                    $('#msglist').html('<tr class="un"><td colspan="4">暂无记录</td></tr>');
                                }
                                else {
                                    html ="";
                                    for (var i = 0; i < data.rows.length; i++) {
                                        var m = data.rows[i];
                                        var cls = "";
                                        if (i % 2 == 1) {
                                            cls = "style=background:#f3f2f2;";
                                        }
                                        var flag = '0';
                                        if (m.state == "UnRead") {
                                            flag = '1';
                                            if (cls == "")
                                                cls = "style=font-weight:bold;";
                                            else
                                                cls += "font-weight:bold";
                                        }
                                        html += ("<tr " + cls + " flag=\"" + flag + "\" id=\"" + m.msg_id + "\"><td><a href=\"javascript:void(0)\" act=readMsg>" + m.title + "</a></td><td>" + m.msg_type + "</td><td>" + m.ctime + "</td><td><a href=\"javascript:void(0)\" act=deleteMsg>删除</a></td></tr>");
                                    }
                                    $('#msglist').html(html);
                                }
                            }
                        }
                        $(c).remove();
                    })
                }
            });
        },
        checkPwd: function (e) {
            if (t(e).attr('readonly') == undefined) {
                var v = (t(e).val() || '').trim();
                var r = t(e).attr('rel');
                if (r != undefined) {
                    var a = (t(r).val() || '').trim();
                    if (v.length == 0) {
                        t(t(e).next()).removeClass('gray').addClass('red');
                        t(t(e).next()).html('<i class="ico ico-error2 "></i>' + t(e).attr('error'));
                        return false;
                    }
                    else {
                        if (v != a) {
                            t(t(e).next()).removeClass('gray').addClass('red');
                            t(t(e).next()).html('<i class="ico ico-error2 "></i>两次输入的密码不一致');
                            return false;
                        }
                    }
                } else {
                    if (v.length < 6) {
                        t(t(e).next()).removeClass('gray').addClass('red');
                        t(t(e).next()).html('<i class="ico ico-error2 "></i>' + t(e).attr('error'));
                        return false;
                    }
                }
                t(t(e).next()).html('');
            }
            return true;
        },
        checkForms: function (e) {
            var v = t(e).val() || '';
            if (v == '') {
                t(t(e).next()).removeClass("gray").addClass('red');
                t(t(e).next()).html('<i class="ico ico-error2 "></i>请输入' + t(e).attr('t-name'));
                return false;
            }
            else {
                var rel = t(e).attr('rel');
                if (rel != undefined) {
                    var v1 = t(rel).val();
                    if (v != v1) {
                        t(t(e).next()).removeClass("gray").addClass('red');
                        t(t(e).next()).html('<i class="ico ico-error2 "></i>两次输入的银行卡号不一致');
                        return false;
                    }
                }
                t(t(e).next()).addClass("gray").removeClass('red');
                t(t(e).next()).html('');
            }
            return true;
        },
        checkUSelect: function (e) {
            var rel = t(e).attr('rel'), tip = t(e).attr('tip');
            if (rel != undefined) {
                var v = t(rel).val() || '';
                if (v == '') {
                    t(tip).removeClass('f-dn')
                    return false;
                }
                else {
                    t(tip).addClass('f-dn')
                }
            }
            return true;
        },
        prepare_cz: function () {

        },
        getQR: function () {
            core.isLogin(function (e) {
                if (e && e.flag != 'y') {
                    core.easyNav.login();
                }
                else {
                    var c = t('#channel').val() || '', p = t('#payid').val() || '', a = t('#amount').val() || '';
                    if (p == '') {
                        t.dialog({ title: '温馨提示', content: '请先选择支付二维码！', animate: 10, width: 385, height: 180 });
                    } else if (a == '') {
                        t.dialog({ title: '温馨提示', content: '请先输入充值金额！', animate: 10, width: 385, height: 180 });
                    } else if (a < 10) {
                        t.dialog({ title: '温馨提示', content: '充值金额不能小于10元！', animate: 10, width: 385, height: 180 });
                    } else {
                        t('div.qr-header').find('span.f-red1').text(t('#username').val());
                        t('img.qr-img').attr('src', this.cdnUrl + "member/getQRImage?u=" + t('#username').val() + "&c=" + c + "&p=" + p + "&a=" + a + "&v=" + +new Date());
                        t.dialog({
                            title: '手机扫一扫',
                            content: t('div.qr-container.f-dn').html(),
                            css: "iDialogToast",
                            animate: 10,
                            width: 450,
                            height: 400,
                            callback: function (e) {
                                t('div.qr-header').find('span.f-red1').text('');
                                t('img.qr-img').removeAttr('src');
                                if (e == null || e == undefined)
                                    return;

                                window.location.href = core.cdnUrl + "member/usercenter/zhmx?t=deposit";
                            }
                        });
                    }
                }
            });
        },
        post_cz: function () {
            alert('post_cz');
        },
        post_pwd: function () {
            var v = t('ul.tit-tab>li.active').attr('val') || '';
            if (v != '') {
                var f1, f2, f3;
                f1 = core.checkPwd(t('#c_pass_' + v));
                f2 = core.checkPwd(t('#n_pass_' + v));
                f3 = core.checkPwd(t('#z_pass_' + v));
                if (f1 && f2 && f3) {
                    var a = core.addDisCover();
                    var c = t('#c_pass_' + v).val() || '', n = t('#n_pass_' + v).val() || '', z = t('#z_pass_' + v).val() || '';
                    core.isLogin(function (e) {
                        if (e && e.flag != 'y') {
                            $(a).remove();
                            core.easyNav.login();
                        }
                        else {
                            core.post(core.cdnUrl + 'member/changePwd', { t: v, c_pass: c, n_pass: n }, function (e, r) {
                                $(a).remove();
                                var d = $.parseJSON(r);
                                if (d.flag == 'y') {
                                    t.dialog({
                                        title: '温馨提示',
                                        content: '<div style="margin-left:10px;margin-top:10px;">密码修改成功！</div>',
                                        css: "iDialogToast",
                                        callback: function (e) {
                                            t('#c_pass_' + v).val('');
                                            t('#n_pass_' + v).val('');
                                            t('#z_pass_' + v).val('');
                                        },
                                        animate: 10,
                                        width: 385,
                                        height: 180
                                    });
                                }
                                else {
                                    var tit = "当前登录密码";
                                    if (v == 'pay') {
                                        tit = "当前支付密码";
                                    }
                                    t.dialog({
                                        title: '温馨提示',
                                        content: '<div style="margin-left:10px;margin-top:10px;">密码修改失败，请核对<span style="font-weight:bold;color:#096ed6">[' + tit + ']</span>的输入信息！</div>',
                                        css: "iDialogToast",
                                        animate: 10,
                                        width: 385,
                                        height: 180
                                    });
                                }
                            })
                        }
                    });
                }
            }
        },
        bind_card: function () {
            var f1 = true, f2 = true, f3 = true;
            t('input[required=true]').each(function (e) {
                if (!core.checkForms(this)) {
                    f1 = false
                }
            })
            t('.list').each(function (e) {
                if (!core.checkUSelect(this)) {
                    f2 = false;
                }
            })
            f3 = core.checkPwd(t('#ppass'));
            if (f1 && f2 && f3) {
                core.isLogin(function (e) {
                    if (e && e.flag != 'y') {
                        core.easyNav.login();
                    }
                    else {
                        t.dialog({
                            title: '温馨提示',
                            content: '<div style="margin:10px 15px"><p class="gray fsz12">提款绑定的银行卡开户行必须和您之前注册的实名一致，否则无法提款。请确认信息无误后再提交</p><p style="padding:15px 0 0 30px;color:#666;"><strong>开户姓名：</strong>' + t('#realname').val() + '<br/><strong>提款银行：</strong>' + t('#bankname').text() + '<br/><strong>银行卡号：</strong>' + t('#cardnum').val() + '</p></div>',
                            css: "iDialogToast",
                            animate: 10,
                            callback: function (b) {
                                if (b != null) {
                                    var a = core.addDisCover();
                                    var data = {
                                        c_pass: t('#ppass').val(),
                                        realname: t('#realname').val(),
                                        bankno: t('#bankno').val(),
                                        bankname: t('#bankname').text(),
                                        province: t('#city_list').val(),
                                        province_name: t('#province_name').text(),
                                        city: t('#city').val(),
                                        city_name: t('#city_name').text(),
                                        subbank: t('#bankzh').val(),
                                        cardnum: t('#cardnum').val()
                                    };
                                    core.post(core.cdnUrl + 'member/changeBank', data, function (e, r) {
                                        $(a).remove();
                                        var data = $.parseJSON(r);
                                        if (data.flag == 'y') {
                                            t.dialog({
                                                title: '温馨提示',
                                                content: '<div style="margin-left:10px;margin-top:10px;"><p class="text-center"><i class="ico ico-succ"></i>恭喜，银行卡绑定成功！</p></div>',
                                                css: "iDialogToast",
                                                callback: function (e) {
                                                    window.location.reload();
                                                },
                                                animate: 10,
                                                width: 385,
                                                height: 180
                                            });
                                        }
                                        else {
                                            t.dialog({
                                                title: '温馨提示',
                                                content: '<div style="margin-left:10px;margin-top:10px;">信息提交失败，错误原因：<span style="font-weight:bold;color:#096ed6">[' + data.message + ']</span>。</div>',
                                                css: "iDialogToast",
                                                animate: 10,
                                                width: 385,
                                                height: 180
                                            });
                                        }
                                    });
                                }
                            },
                            width: 385,
                            height: 270
                        });
                    }
                });
            }
        },
        postinfo: function () {
            var f1 = true, f2 = true;
            t('input[required=true]').each(function (e) {
                if (!core.checkForms(this)) {
                    f1 = false
                }
            })
            f2 = core.checkPwd(t('#ppass'));
            if (f1 && f2) {
                core.isLogin(function (e) {
                    if (e && e.flag != 'y') {
                        core.easyNav.login();
                    }
                    else {
                        var a = core.addDisCover();
                        var data = {
                            c_pass: t('#ppass').val(),
                            nickname: t('#nickname').val(),
                            email: t('#email').val(),
                            realname: t('#realname').val(),
                            idtype: t('#idtype').val(),
                            id: t('#ident').val(),
                            ask: t('#ask').val(),
                            answer: t('#answer').val()
                        };
                        core.post(core.cdnUrl + 'member/postMember', data, function (e, r) {
                            $(a).remove();
                            var data = $.parseJSON(r);
                            if (data.flag == 'y') {
                                t.dialog({
                                    title: '温馨提示',
                                    content: '<div style="margin-left:10px;margin-top:10px;">信息提交成功！</div>',
                                    css: "iDialogToast",
                                    callback: function (e) {
                                        window.location.reload();
                                    },
                                    animate: 10,
                                    width: 385,
                                    height: 180
                                });
                            } else {
                                t.dialog({
                                    title: '温馨提示',
                                    content: '<div style="margin-left:10px;margin-top:10px;">信息提交失败，错误原因：<span style="font-weight:bold;color:#096ed6">[' + data.message + ']</span>。</div>',
                                    css: "iDialogToast",
                                    animate: 10,
                                    width: 385,
                                    height: 180
                                });
                            }
                        });
                    }
                })
            }
        },
        postrequest: function (e) {
            var tip = t(e).attr('tip'), v = t(e).attr('val');
            core.isLogin(function (e) {
                if (e && e.flag != 'y') {
                    core.easyNav.login();
                }
                else {
                    t.dialog({
                        title: '温馨提示',
                        content: '<div style="margin-left:10px;margin-top:10px;">' + tip + '？</div>',
                        css: "iDialogToast",
                        callback: function (e) {
                            if (e == null) return;
                            var a = core.addDisCover();
                            core.post(core.cdnUrl + 'member/postRequest/' + v, { msgid: +new Date() }, function (e, r) {
                                $(a).remove();
                                var data = $.parseJSON(r);
                                if (data.flag == 'y') {
                                    t.dialog({
                                        title: '温馨提示',
                                        content: '<div style="margin-left:10px;margin-top:10px;">信息提交成功！</div>',
                                        css: "iDialogToast",
                                        animate: 10,
                                        width: 385,
                                        height: 180
                                    });
                                }
                                else {
                                    t.dialog({
                                        title: '温馨提示',
                                        content: '<div style="margin-left:10px;margin-top:10px;">信息提交失败，错误原因：<span style="font-weight:bold;color:#096ed6">[' + data.message + ']</span>。</div>',
                                        css: "iDialogToast",
                                        animate: 10,
                                        width: 385,
                                        height: 180
                                    });
                                }
                            })
                        },
                        animate: 10,
                        width: 385,
                        height: 180
                    });
                }
            })
        }
    });
}(window, Core, jQuery);