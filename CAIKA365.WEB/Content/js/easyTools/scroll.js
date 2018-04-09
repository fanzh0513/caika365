/*
 * 滚动定位组件
 * 2013-04-28 马超 增加
 * 2014-02-10 马超 增加第一个参数的功能，可以直接传递top值
 * 2014-08-08 郑弋天 增加参数endFn,endTime
 *
 * dom : [必选]要定位参考的dom元素（必须是可视的）或者是要定位的top值
 * force : [可选]是否强制滚动定位，默认flase，如果设置为true，则会强制滚动到指定元素的顶部
 * endFn ：[可选]滚动结束时的回调
 * endTime : [可选]多少毫秒后滚动结束, 固定时间的滚动推荐使用$('html,body').animate({scrollTop:0}, 1000, function(){})
 */
(function(window,$){
var scrollTimer, win = $(window);
$.scrollWhenNeed = function( domOrTop, force, endFn, endTime ){
	//先终止未完成的滚动
	scrollTimer && window.clearTimeout(scrollTimer);
	scrollTimer = null;
	//不需要滚动的先处理
	var hook, top, _top, 
		// 间隔时间
		perTime = 10,
		// 每次移动的top
		perTop = 20
	;
	if( !isNaN(domOrTop) ){ //是数字
		top = parseInt(domOrTop);
	}else{
		hook = $(domOrTop);
		if( !hook[0] ) return this;
		top = hook.offset().top;
	}
	_top = win.scrollTop();
	if(force && _top == top) return this;
	if( !isNaN(endTime) ){
		perTop = Math.floor(Math.abs(top - _top)/(endTime/perTime));
	}
	//如果需要或强制，则滚动
	if( force || _top < top ){
		var dir = top > _top, move = function(){
			scrollTimer = window.setTimeout(function(){
				_top += dir? perTop : -perTop;
				win.scrollTop( _top );
				//判断是否移动到位
				if( Math.abs(top - _top) < perTop ){
					win.scrollTop( top );
					scrollTimer = null;
					endFn && endFn.call(this);
				}else{
					move();
				}
			}, perTime);
		};
		move();
	}
	return this;
};

/*
 * 数据滚动组件
 * 2012-05-03 马超 创建
 * 支持一个组合参数：
 *	perScroll	每次滚动的行数
 *	speed		每移动一个像素平均花费的时间（ms）
 *	interval	两次滚动之间的时间（含滚动时间）
 */
 // 注：li标签需要有高度，如果子元素浮动，li标签应该加 .clearfix
$.fn.scrollGrid = function( option ){
	var op = $.extend({
		perScroll : 3,
		speed : 14,
		interval : 5000
	}, option||{});
	//逐个进行处理
	return this.each(function(){
		var box = $(this), ul = box.find(">ul"), perScroll = op.perScroll, odd = ul.find("li").length%2, timer,
		action = function(){
			//动态计算需要移动的高度
			var height=0, orgLi = ul.find("li:lt("+ perScroll +")").each(function(){ height += $(this).outerHeight() }),
			//修复奇偶行的样式
			copyLi = orgLi.clone().removeClass("odd even").appendTo(ul).each(function(){ odd = (odd+1)%2; $(this).addClass( odd ? "odd" : "even" );  });
			//动画进行移动
			ul.animate({marginTop:"-"+ height +"px"}, height*op.speed, function(){
				orgLi.remove();
				ul.removeAttr("style");
			});
		};
		//检查外容器是否必要滚动
		if( ul.length == 1 && box[0].scrollHeight > box[0].offsetHeight ){
			box.bind({
				mouseenter : function(){ window.clearInterval(timer); },
				mouseleave : function(){ timer = window.setInterval(action, op.interval); }
			});
			box.mouseleave();
		}
	});
};
/*
 * 数据滚动组件，2013-06-06 lfp
 * marRight		每一条后面的间距，默认为30,（即li的间距） 
 * WaitTime		滚动一次停留的时间，默认是3s
 * speed		滚动的速度，默认是slow
 */
$.fn.xScrollGrid = function( marRight, WaitTime, speed){
var marRight = marRight||30, WaitTime = WaitTime||3000, speed = speed||"slow";
	return this.each(function(){
		var wVal=0, box = $(this), ul = box.find("ul").eq(0), inter,
		scr = function(){
			var fistLi = ul.find("li").eq(0);
			ul.animate({left : -(ul.find("li").eq(0).width()+marRight)+"px"}, speed, function(){
				ul.append(fistLi);
				ul.css("left" , "0px");
			});
		};
		$.each(ul.find("li"), function(){
			//动态计算ul中所有li的宽度之和
			wVal += $(this).width()+marRight;
		});
		if(wVal < box.width()){ return; }
		ul.mouseenter(function(){
			inter && window.clearInterval(inter);
		}).mouseleave(function(){
			inter = setInterval(function(){
				scr();
			}, WaitTime);
		});
		ul.mouseleave();
	});
}
})(window,jQuery);
