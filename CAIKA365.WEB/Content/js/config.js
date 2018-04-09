!function (e, r) {
    r.gameActConfig = {
        jxd11: {
            type: ["jsj", "hot2"],
            range: "server"
        },
        dlt: {
            type: "jiajiang",
            range: "server"
        },
        jclq: {
            type: ["NBA", "w200s20"],
            range: "server"
        },
        jxssc: {
            type: "jsj",
            range: "server"
        },
        x3d: {
            type: "jsj",
            range: "server"
        },
        football_sfc: {
            type: "jiajiang",
            range: "server"
        },
        football_9: {
            type: "jiajiang",
            range: "server"
        },
        d11: {
            type: "jsj",
            range: "server"
        },
        gdd11: {
            type: "jsj",
            range: "server"
        },
        hljd11: {
            type: "jsj",
            range: "server"
        },
        oldkuai3: {
            type: "jsj",
            range: "server"
        },
        ssq: {
            type: "3yiJiajiang",
            range: "server"
        },
        mobile: {
            type: "mobileNew",
            range: "server"
        },
        gxkuai3: {
            type: "jsj",
            range: "server"
        },
        kl8: {
            type: "jiajiang",
            range: "server"
        },
        lnd11: {
            type: "jiajiang",
            range: "server"
        }
    };
    e(document).ready(function () {
        r.gameAct.is("ssq", "3yiJiajiang",
        function (e) {
            if (e) r.loadCdnCss("/res/css/activity/jiajiang/ssq3yiJj20131022.css")
        });
        r.gameAct.is("jclq", "NBA",
        function (r) {
            if (r) {
                var a = e(".cz_intro");
                var s = "cz_NBA";
                a.find("[gid='jclq']").append("<span class='" + s + "'></span>")
            }
        })
    })
}(jQuery, Core);