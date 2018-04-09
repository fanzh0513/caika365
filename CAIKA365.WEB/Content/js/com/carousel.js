/*
    @fileOverview 轮播组件
    @author 王炜毅
   
    [change log]
    2014-09-20 v1.0   王炜毅 创建
    2014-10-10 v1.0.1 王炜毅 添加debug配置项；代码细节修正： 1.fixIdx()方法中substitue未找到时跳转到第一个；2.destroy()细节修正
    2014-10-16 v1.0.2 王炜毅 去掉触屏相关判断和处理，今后若有需要再专门抽出一个wap版的

    [api]
    $.fn.carousel([opts]); 注册轮播组件
    @this 会被当作轮播容器
    @param {Object} opts 配置项,将覆盖同名的默认配置, 其中dom配置项接受3种形式: jquery对象 | dom对象 | jquery选择器
    @return Carousel对象
    
    实例方法: 
    var carouselObj = $.fn.carousel([opts]);
    carouselObj.prev(); 切换上一个/上一组(step>1时)
    carouselObj.next(); 切换下一个/下一组(step>1时)
    carouselObj.go(to); 切换到下标为to的元素
    注: 一般情况下建议配置prevBtn, nextBtn, goBtnsWrap等来操纵切换, 而不是手动调用这些方法.
    
    carouselObj.resize() 当轮播元素尺寸变化时需要调用此方法来通知组件重新计算(无需传递元素宽度值,组件会自动计算).
    carouselObj.startAuto() 开始自动轮播
    carouselObj.stopAuto() 停止自动轮播
    carouselObj.destroy() 注销, 会清除组件绑定在dom元素上的所有数据和事件监听
 */
(function($, undefined) {
'use strict';
var 
    // 当前浏览器中Css3 Animation的结束事件名
    animEndEvtName = (function(){
        var prop = 'animation',
            styleObj = document.createElement('div').style,
            cssPrefixes = ['Webkit', 'Moz', 'O', 'ms'],
            ucProp = prop.charAt(0).toUpperCase() + prop.slice(1),
            props = (prop + ' ' + cssPrefixes.join(ucProp + ' ') + ucProp).split(' '),
            map = {
                'WebkitAnimation': 'webkitAnimationEnd',
                'OAnimation': 'oAnimationEnd',
                'msAnimation': 'MSAnimationEnd',
                'animation': 'animationend'
            };

        for (var i in props) {
            if (styleObj[props[i]] !== undefined) {
                return map[props[i]];
            }
        }
        return '';
    })(),

    //是否支持Css3 Animation
    supportCss3 = animEndEvtName !== '',

    //js动画模式, 兼容所有浏览器
    jsAnimations = ['show', 'fade', 'slide'],

    //css3动画模式（兼容IE10+, FF5+, Chrome4+, Safari4+, iOS Safari3.2+, Opera12+, Android2.3+）
    css3Animations = ['fxFade', 'fxCorner', 'fxVScale', 'fxFall', 'fxFPulse', 'fxRPulse', 'fxHearbeat', 
        'fxRotateSoftly', 'fxDeal', 'fxFerris', 'fxShinkansen', 'fxSnake', 'fxShuffle', 'fxPhotoBrowse', 'fxCoverflow',
        'fxSlideBehind', 'fxVacuum', 'fxHurl'],

    //默认配置
    defaults = {
        debug: false,       //是否为debug模式，设置为true时将显示错误信息（如果有的话）
        animate: 'slide',   // 动画模式, 可选值为 jsAnimations 或 css3Animations数组中的任意一项

        //---- animate: 'slide' 专有配置项 ----//
        list: 'ul',     // 轮播元素列表, animate='slide'时为<必选>. 以wrap.find(list)的方式查找list元素.
                        //  dom要求: 
                        //      animate == 'slide': div>ul>li, 其中div为this(轮播容器), ul为list, li为轮播元素
                        //      animate != 'slide': ul>li, 其中ul为this(轮播容器), li为轮播元素
                        //  注: 上面只是举例, 项目中可用任意标签, 如div, p, section,span等等均可, 只要dom结构符合上述要求即可
                                
        vertical: false, // 是否为垂直滚动
        step: 1,         // 切换跨, 即一次滚动几个元素
        visible: 1,      // 可见元素个数, 不配置时默认与step相同
        clone: true,     // 是否复制节点以实现循环, 为true时从最后一个节点切换到第一个节点滚动方向不变;
                         // 为false时不复制节点, 从最后一个节点切换到第一个节点时与滚动方向相反(实际是拉回到初时位置).
                         // 有些情况下是不能复制节点的, 如轮播元素是投注模块, 则在A模块中选中了某个号码, 
                         // 在A.clone模块中无法同步,会造成数据紊乱. 这种情况下可设置clone:false阻止复制。
        //---- End专有配置项 ----//
        
        prevBtn: '',            // '上一个'切换按钮, 通过$(prevBtn)方式查找
        nextBtn: '',            // '下一个'切换按钮, 通过$(nextBtn)方式查找
        goBtnsWrap: '',         // 跳转按钮容器, 通过$(goBtnsWrap)方式查找. 其直接子元素会被当作跳转按钮。 
                                // 跳转按钮与轮播元素通过下标一一对应，请保证二者数量上一致，组件对此未作校验。
        prevTriggerEvt: 'click',        // '上一个'触发事件
        nextTriggerEvt: 'click',        // '下一个'触发事件
        goBtnTriggerEvt: 'mouseenter',  // 跳转按钮触发事件, 可选值 'click' | 'mouseenter', 触屏设备上将强制重置为'click'
        goBtnActiveClass: 'active',     // 跳转按钮被选中时的样式

        speed: 1000,            // 切换动画执行一次的总时长
        auto: true,             // 是否自动切换下一个
        autoInterval: 3000,     // 自动切换间隔
        start: 0,               // 从第几个元素开始显示, 按下标计算，即第一元素为0，第n个为n-1
        cssPath: '../index/carousel.css',  //carousel.css路径

        complete: function(carouselObj) {},   // 初始化完成回调
        // @param {Carousel} carouselObj 对应的carousel对象
        // 在通过bindMoudle异步加载carousel.js的情况下, 通过$.fn.carousel()注册组件时Carousel对象不会立即返回, 
        // 此时需要在回调方法中得到Carousel对象后再进行相应操作
        before: function(curItem, curGoBtn, prevBtn, nextBtn) {}, // 切换前回调, 参数同after
        after: function(curItem, curGoBtn, prevBtn, nextBtn) {}  // 切换完成后的回调
        // @param {jqueryObject} curItem 当前轮播元素
        // @param {jqueryObject} curGoBtn 当前跳转按钮
        // @param {jqueryObject} prevBtn '上一个'按钮, 未设置时此值为null
        // @param {jqueryObject} nextBtn '下一个'按钮, 未设置时此值为null
    };

    /**
     * @constructor Carousel 
     * @param {Object} opts 配置参数
     * @example new Carousel({wrap: '.itemWrap', start:2, complete: function() {console.log('init complete!')}});  
     */
    function Carousel(opts) {
		var cache = this.cache = $.extend({}, defaults, opts);
        //检验数据并初始化
        if(this._check()) {
            this._prepareData(opts.visible)._backupStyle()._prepareSlide()._loadCss()._bindEvent()
                .resize() //调整dom尺寸为轮播作准备
                .go(cache.start, true); //无动画切换到第start个轮播无素
        }   
        //初始化完成回调
        cache.complete(this); 
    }

    Carousel.prototype = {
        constructor: Carousel,
        carousel: '1.0.2', 
        //检查必要数据合法性
        _check: function() {
            var cache = this.cache, 
                animate = cache.animate,  
                wrap = $(cache.wrap), 
                list = cache.list.jquery ? cache.list : wrap.find(cache.list),
                isSlide = animate === 'slide',
                //去掉带clone标记的元素, 防止重复调用造成元素循环复制
                items = isSlide ? list.children().filter(":not(.clone)") : wrap.children().filter(":not(.clone)"),
                itemCnt = items.length,
                showError = cache.debug ? (window.console && window.console.log ? window.console.log : window.alert) : $.noop;

            if (!wrap.length) {
                showError('轮播容器不存在');
                return false;

            } else if (isSlide && !list.length) {
                showError('slide模式下list不存在');
                return false;

            } else if (itemCnt < 2) {
                showError('轮播元素数量小于2');
                return false;

            } else if((supportCss3 && $.inArray(animate, jsAnimations.concat(css3Animations)) === -1) || 
                (!supportCss3 && $.inArray(animate, jsAnimations) === -1)){
                showError('无效的动画模式');
                return false;
            }  

            $.extend(cache, {
            	isSlide : isSlide, 
            	wrap: wrap,
            	list: isSlide ? list : null,
            	items: items,
            	itemCnt: itemCnt
            });

            return true;
        },

        //修正不合法数据
        _prepareData: function(optVisible) {
            var cache = this.cache, itemCnt = cache.itemCnt;

            cache.step = cache.isSlide && +cache.step > 1 ? (+cache.step > itemCnt-1 ? itemCnt-1 : Math.floor(+cache.step)) : 1;
            cache.visible = +optVisible ? (+optVisible < 1 ? 1 : (+optVisible > itemCnt-1 ? itemCnt-1 : Math.floor(optVisible))) : cache.step;
            cache.speed = +cache.speed >= 100 ? +cache.speed : 1000;
            cache.start = (+cache.start >= 0 && +cache.start < itemCnt) ? Math.floor(cache.start) : 0;
            cache.autoInterval = +cache.autoInterval > 100 ? +cache.autoInterval : 3000;
            cache.cssPath = cache.cssPath || '../css/carousel.css';
            cache.itemIdx = cache.start; //当前显示的轮播元素的下标
            cache.running = false; //是否正在执行切换动画
            cache.animFn = $.inArray(cache.animate, jsAnimations) > -1 ? '_' + cache.animate + 'To' : '_animateTo';
            
            //$(new Date| new Object).length为1，为避免这种情况，另外再加上selector的判断
            cache.prevBtn = $(cache.prevBtn).length && $(cache.prevBtn).selector ? $(cache.prevBtn): null;
            cache.nextBtn = $(cache.nextBtn).length && $(cache.nextBtn).selector ? $(cache.nextBtn): null;
            cache.goBtnsWrap = $(cache.goBtnsWrap).length && $(cache.goBtnsWrap).selector ? $(cache.goBtnsWrap): null;
            cache.goBtns = cache.isSlide && +cache.step > 1 ? null : //slide multi scroll模式下不支持跳转按钮
                            cache.goBtnsWrap && cache.goBtnsWrap.children().length ? $(cache.goBtnsWrap).children() : null;
            cache.goBtnTriggerEvt = cache.goBtnTriggerEvt === 'click' ? 'click' : 'mouseenter';

            cache.before = $.isFunction(cache.before) ? cache.before : $.noop;
            cache.after = $.isFunction(cache.after) ? cache.after : $.noop;
            cache.complete = $.isFunction(cache.complete) ? cache.complete : $.noop;
            return this;
        },

        //备份元素初始的内联样式，以便注销时恢复到该初始状态
        _backupStyle: function() {
            var cache = this.cache, itemsStyle, itemsClass, goBtnsClass;

            cache.backup = {};
            itemsStyle = cache.backup.itemsStyle = []; 
            itemsClass = cache.backup.itemsClass = []; //不能和上一行连等，是不同的array对象！
            
            cache.backup.wrapStyle = cache.wrap[0].style.cssText;
            cache.backup.wrapClass = cache.wrap.attr('class');

            if(cache.isSlide) {
                cache.backup.listStyle = cache.wrap[0].style.cssText;
                cache.backup.listClass = cache.list.attr('class');
            } 
            
            cache.items.each(function(i, item) {
                itemsStyle.push(item.style.cssText);
                itemsClass.push($(item).attr('class'));
            });

            if(cache.goBtns) {
                goBtnsClass = cache.backup.goBtnsClass = [];
                cache.goBtns.each(function(i, goBtn) {
                    goBtnsClass.push($(goBtn).attr('class'));
                });
            }
            return this;
        },

        //准备slide动画所需dom和数据
        _prepareSlide: function() {
            var cache = this.cache, items = cache.items;

            if(cache.isSlide) {  
                if(cache.clone) {
                    //清空列表，以防止重复注册时造成元素重复复制
                    cache.list.empty();
                    //记录元素在原列表中的下标, 以计算循环
                    items.each(function(i, item) {
                        $(item).data('origin-idx', i);
                    });
                    //复制节点
                    cache.list.append(items.clone(true).addClass('clone')).append(items).append(items.clone(true).addClass('clone'));
                    items = cache.list.children();
                    //udpate data
                    cache.items = items;
                    cache.itemCnt = items.length;
                } 
                if(cache.vertical) {
                    cache.animProp = 'margin-top';
                    items.css({'float': 'none'});
                } else {
                    cache.animProp = 'margin-left';
                    items.css({'float': 'left'});
                }                
                //ensure neccesary style
                cache.wrap.css('overflow', 'hidden');
            }
            return this;
        },

        //加载样式表
        _loadCss: function() {
            var cache = this.cache, animate = cache.animate;
            if ( !/slide|show/.test(animate)) {
                !$('#carouselStyle').length && $('<link>').attr({id: 'carouselStyle', type: 'text/css', rel: 'stylesheet', href: cache.cssPath }).appendTo($('head')[0]);
                //1500ms若css文件加载完毕，或页面中本就有这个css文件，添加相应的class
                setTimeout(function() {
                    if($('#carouselStyle').length) {
                        cache.wrap.addClass(/^fx/.test(animate) ? 'carouselWrap-css3 ' + animate : 'carouselStage-jsFade');
                        cache.items.addClass('carouselItem');
                    }
                }, 1500); 
            } 
            return this;
        },

        //绑定事件监听
        _bindEvent: function() {
            var cs = this, cache = this.cache, goBtns = cache.goBtns, 
                timeStart = 0, timer = -1, jumpFun, touchStart, touchEnd, touchDelta;

            //上一个/下一个
            cache.prevBtn && cache.prevBtn.unbind(cache.prevTriggerEvt).bind(cache.prevTriggerEvt, $.proxy(this.prev, this));
            cache.nextBtn && cache.nextBtn.unbind(cache.nextTriggerEvt).bind(cache.nextTriggerEvt, $.proxy(this.next, this));

            //切换到与当前goBtn下标相同的轮播元素
            if (goBtns) {
                jumpFun = function() {
                    var goIdx = $(this).index();
                    
                    if(cache.isSlide && cache.clone) {//找到距离最近的目标节点
                        var itemIdx = cache.itemIdx,
                            originIdx = +cache.items.eq(itemIdx).data('origin-idx');
                        goIdx += itemIdx - originIdx;
                    }

                    cs.go(goIdx * cache.step);
                };

                if (cache.goBtnTriggerEvt === 'click') {
                    goBtns.unbind("click", jumpFun).click(jumpFun);

                } else { //mouseenter设置延时200ms,避免鼠标无意间划过而引起切换

                    goBtns.unbind("mouseenter mouseleave").mouseenter(function() {
                        timeStart = +new Date(); //记录开始时间
                        timer = window.setTimeout($.proxy(jumpFun, this), 200);

                    }).mouseleave(function() {
                        (+new Date() - timeStart < 200) && window.clearTimeout(timer);
                    });
                }
            }

            //鼠标在轮播元素或goBtns上悬停时停止自动轮播
            cache.items.unbind("mouseenter mouseleave").add(cache.goBtns)                
                .mouseenter(function() {
                    cache.isHovering = true;
                    cache.auto && cs.stopAuto();
                }).mouseleave(function() {
                    cache.isHovering = false;
                    cache.auto && cs.startAuto();
                });
          
            return this;
        },

        //修正即将切换到的元素的下标
        _fixIdx: function(to) {
            var cache = this.cache, itemCnt = cache.itemCnt, itemIdx = cache.itemIdx,
                items = cache.items, originIdx, maxTo, substitute, idx;

            if (cache.isSlide && cache.clone) {
                originIdx = +items.eq(itemIdx).data('origin-idx') || 0;
                maxTo = itemCnt - cache.visible;  
                
                if (to > maxTo) {
                    substitute = items.filter(function() {
                        return $(this).data('origin-idx') === originIdx;
                    }).first();
                    //update on 2014-10-10
                    //发现在aside.js中，最新中奖数据会在每次获取到开奖后，刷新dom节点：rankDom.recent.html(tmpl.getRecent(json.recent));
                    //这样会删除本组件clone的节点，且使cache.list的长度也不对了， 在这种情况下， substitute也可能会找不到
                    //虽然aside.js已经修正了，但未防止类似的情况出现，现添加substitue未找到时的兼容方案
                    idx = substitute.length ? substitute.index() : 0;
                    to = idx + cache.step;
                    //移动列表
                    this._slideTo(idx, true); 

                } else if (to < 0) {
                    substitute = items.filter(function(i) {
                        return $(this).data('origin-idx') === originIdx && i <= maxTo && i >= cache.step;
                    }).last();
                    idx = substitute.length ? substitute.index() : maxTo;
                    to = idx - cache.step;
                    //移动列表
                    this._slideTo(idx, true); 
                } 
                return to;

            } else {
                return to > itemCnt-1 ? to%itemCnt : (to < 0 ? (to%itemCnt == 0 ? 0 : to%itemCnt+itemCnt) :to);
            }
        },

        //show/hide方式切换
        _showTo: function(to, callback) {
            to = this._fixIdx(to);
            this.cache.items.hide().eq(to).show();
            callback(to);
            return this;
        },
        
        //淡出效果
        //@param noAnimation: 无动画切换
        _fadeTo: function(to, callback, noAnimation) {
            to = this._fixIdx(to);
            var cache = this.cache,
                items = cache.items,
                curItem = items.eq(cache.itemIdx),
                toItem = items.eq(to).addClass('switching'); //提升zindex以保证当前元素淡出后显示的一定是它

            if(noAnimation) {
            	callback(to);
            	return this;
            }

            curItem.fadeOut(cache.speed, function() {
                //淡出算法最后会将元素隐藏(display: none)，为保证下次切换到该元素时可以正常显示, 要将其显示出来
                curItem.show();
                toItem.removeClass('switching');
                callback(to);
            });
            return this;
        },

        //滑动效果
        _slideTo: function(to, callback, noAnimation) {
            var cache = this.cache, prop = {};

            to = this._fixIdx(to);
            prop[cache.animProp] = -cache.animSize*to +'px';

            if(!$.isFunction(callback)) { //for _fixIdx()
                noAnimation = callback;
                callback = $.noop;
            } 
           
            if(noAnimation) {
            	cache.list.css(prop);
            	callback(to);
            	return this;
            }
           
            cache.list.animate(prop, cache.speed, function() {
                callback(to);
            });
            return this;
        },

        //css3 animation动画方式，通过carousel.css实现(由组件自动加载)
        _animateTo: function(to, callback, noAnimation) {
            var cache = this.cache,
                itemIdx = cache.itemIdx,
                inClass = to > itemIdx ? 'navInNext' : 'navInPrev',
                outClass = to > itemIdx ? 'navOutNext' : 'navOutPrev',
                curItem = cache.items.eq(itemIdx),
                newTo = this._fixIdx(to),
                toItem = cache.items.eq(newTo),
                cntAnim = 0,
                animEndFun = function() {
                    cntAnim++;
                    if (cntAnim === 2) { //动画完成
                        curItem.removeClass(outClass);
                        toItem.removeClass(inClass);
                        callback(newTo);
                    }
                };

            if(noAnimation) {
            	callback(newTo);
            	return this;
            }

            curItem.one(animEndEvtName, animEndFun).addClass(outClass);
            toItem.one(animEndEvtName, animEndFun).addClass(inClass);
            return this;
        },
         
        /**
         * @description 轮播元素尺寸发生变化时的通知事件。某些情况下, 轮播元素的宽高会发生变化，此时需通知组件以调整算法。
         * @return {Carousel} this
         */        
        resize: function() {
            var cache = this.cache, wrap = cache.wrap, list = cache.list, items = cache.items,
                ow = items.outerWidth(true), oh = items.outerHeight(true),
                visible = cache.visible, itemCnt = cache.itemCnt, 
                halfMgb, animSize, wrapCssText, listCssText, itemCssText;

            if(cache.isSlide) {
                if (cache.vertical) {
                    //因垂直margin会合并，为保证每次移动的距离相同，将轮播元素的marginTop设为marginBottom的一半
                    halfMgb = parseInt(items.css('margin-bottom'));
                    animSize = oh - halfMgb; 
                    
                    items.css({'margin-top': halfMgb + 'px'}).last().css('margin-bottom', halfMgb + 'px');
                    wrap.height(animSize * visible); //只修改动画所依赖的属性
                    list.height(animSize * itemCnt);

                } else {
                    animSize = ow;
                    wrap.width(animSize * visible);//只修改动画所依赖的属性
                    list.width(animSize * itemCnt);
                }
                cache.animSize = animSize;

            } else {
                cache.wrap.height(oh).width(ow);
            }
            return this;
        },

        /**
         * @description 切换上一个
         * @return {Carousel} this
         */
        prev: function() {
            return this.go(this.cache.itemIdx - this.cache.step);
        },

        /**
         * @description 切换下一个
         * @return {Carousel} this
         */
        next: function() {
            return this.go(this.cache.itemIdx + this.cache.step);
        },

        /**
         * @description 切换到指定元素
         * @param {Number} to 要切换到的元素的索引
         * @param {Boolean} noAnimation 若为true则不显示动画过程,直接完成切换
         * @return {Carousel} this
         */
        go: function(to, noAnimation) {
            var cs = this, cache = this.cache, activeCls = cache.goBtnActiveClass, 
                itemIdx = cache.itemIdx, curItem = cache.items.eq(itemIdx);

            //若当前正在执行动画，或要切换的元素即为当前元素, 或before()的返回值为false，返回
            if (cache.running
                 || (to === (cache.isSlide && cache.clone ? +curItem.data('origin-idx') : itemIdx) && !noAnimation) //初始化时start=itemIdx, 为了标注当前元素需要走一次
                 || this.cache.before(curItem, cache.goBtns ? cache.goBtns.filter(activeCls) : null, cache.prevBtn, cache.nextBtn) === false) {
                return;
            }

            cache.running = true;
            cache.auto && this.stopAuto();
            cache.goBtns && cache.goBtns.removeClass(activeCls);

            return this[cache.animFn](to, function(newTo) { //to若小于0或超出itemCnt-1，则会在动画函数中被重写
                var goBtnIdx = cache.isSlide && cache.clone ? (+cache.items.eq(newTo).data('origin-idx') / cache.step): newTo,
                    curGoBtn = cache.goBtns ? cache.goBtns.eq(goBtnIdx).addClass(activeCls) : null,
                    curItem = cache.items.removeClass('active').eq(newTo).addClass('active');
                
                cache.itemIdx = newTo;
                cache.auto && !cache.isHovering && !cache.isTouching && cs.startAuto(); //当鼠标或手势在元素上悬停时，不恢复自动轮播
                cache.after(curItem, curGoBtn, cache.prevBtn, cache.nextBtn);            
                cache.running = false; //释放锁
            }, noAnimation);
        },

        /**
         * @description 开始自动轮播
         * @return {Carousel} this
         */
        startAuto: function() {
            this.stopAuto(); //避免连续调用多次时造成多个计时器在跑
            this.cache.autoTimer = window.setInterval($.proxy(this.next, this), this.cache.autoInterval);
            this.cache.auto = true; //若初始化后手动调用此方法，则将auto状态同步为true
            return this;
        },
        
        /**
         * @description 停止自动轮播
         * @return {Carousel} this
         */
        stopAuto: function() {
            this.cache.autoTimer && window.clearInterval(this.cache.autoTimer);
            return this;
        },

        /**
         * @description 注销组件
         * @return {Carousel} this
         */
        destroy: function() {
            var cache = this.cache, 
                backup = cache.backup,
                itemsStyle = backup.itemsStyle, 
                itemsClass = backup.itemsClass, 
                goBtnsClass = backup.goBtnsClass,
                wrap = cache.wrap, list = cache.list, items = cache.items, goBtns = cache.goBtns;

            //stop auto
            cache.autoTimer && window.clearInterval(cache.autoTimer);
            
            //unbind event-handler
            cache.prevBtn && cache.prevBtn.unbind(cache.prevTriggerEvt);
            cache.nextBtn && cache.nextBtn.unbind(cache.nextTriggerEvt);
            items.unbind("mouseenter mouseleave touchstart touchend")
            goBtns && goBtns.unbind(cache.goBtnTriggerEvt + cache.auto ? ' mouseenter mouseleave' : '')

            //restore to initial state
            goBtns && goBtns.each(function(i, goBtn) {
                goBtnsClass[i] ? $(goBtn).attr('class', goBtnsClass[i]) : $(goBtn).removeAttr('class');
            });

            backup.wrapClass ? wrap.attr('class', backup.wrapClass) : wrap.removeAttr('class');
            backup.wrapStyle ? (wrap[0].style.cssText = backup.wrapStyle) : wrap.removeAttr('style');
            if(cache.isSlide) {
                backup.listClass ? list.attr('class', backup.listClass) : list.removeAttr('class');
                backup.listStyle ? (list[0].style.cssText = backup.listStyle) : list.removeAttr('style');
            } 

            cache.items.filter('.clone').remove(); //remove cloned items
            wrap.children().each(function(i, item) {  //only operate uncloned items
                var me = $(item);
                itemsStyle[i] ? (item.style.cssText = itemsStyle[i]) : me.removeAttr('style');
                me.removeData('origin-idx').attr('class', itemsClass[i]);
            });
            
            //destory cache
            cache = this.cache = null;

            //can not invoke methods any more
            this._check = this._bindEvent = this._prepareData = this._loadCss 
            = this._showTo = this._fadeTo = this._slideTo = this._animateTo 
            = this.next = this.prev = this.go = this.stopAuto = this.startAuto = this.destroy = $.noop; 
            
            return this;
        }
    };

    $.fn.carousel = function(opts) {
        //因prevBtn, nextBtn, goBtns等正常情况下是不共用的,所以暂时不支持批量注册
        return new Carousel($.extend(opts, {wrap : this[0]}) );
    };
})(window.jQuery);
