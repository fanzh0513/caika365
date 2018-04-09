/*
 * easyTools 工具、小组件集合
 *
 * 所有该文件中的工具或组件，需要满足以下几个条件：
 * 1、功能必须是异步类型的
 * 2、不用加载额外单独的css文件
 * 3、比较常用，并且在easyCore中已经注册为自动加载模块
 *
 * 不符合以上要求的，请单独创建js文件，并通过core上的自动模块加载进行加载和使用
 *
 */
(function($){
/*
 * jQuery原型扩展
 */
$.fn.extend({
	// 设置选择限制 / 取消选择限制
	disableSelection: function() {return this.attr('unselectable', 'on').css('MozUserSelect', 'none').bind('selectstart', $.falseFn);},
	enableSelection: function() {return this.removeAttr('unselectable').css('MozUserSelect', '').unbind('selectstart').bind('selectstart', $.stopProp);},
	// 禁止右键 / 开启右键
	disableRightClick: function(){ return this.bind("contextmenu", $.falseFn); },
	enableRightClick : function(){ return this.unbind("contextmenu", $.falseFn).bind("contextmenu", $.stopProp); },
	//禁止/启用输入法
	disableIME : function(){ return this.css("ime-mode", "disabled"); },
	enableIME : function(){ return this.css("ime-mode", ""); }
});

/*
 * 设置元素多态样式，normal状态是默认状态，不予以特殊标记；link的hover动作通过CSS设定，JS不参与
 * downCSS		鼠标按下样式，默认 down
 * keepDownCss	鼠标点击后保持的样式，如果没有按下，则不用此参数
 */
$.fn.setControlEffect = function( downCss, keepDownCss ){
	return this.each(function(){
		//添加一次性标志位
		if( this.bindControlEffect )return;
		this.bindControlEffect = 1;
		//检测并绑定
		var down = downCss || "down", fix;
		//2011-05-30 增加对down样式的记忆功能，以便和disabled成对绑定
		if( /^down(.+)$/.test(down) )
			fix = RegExp.$1;
		fix !== undefined && (
			this.bindDownCssFix = fix,
			$(this).hasClass("disabled") && $(this).removeClass("disabled").addClass("disabled"+fix)
		);
		//按下状态
		//IE(<9)下连续高速点击，将导致一部分mousedown事件丢失
		//IE 6 7 8  左键 1 右键2 中键 4  若是组合键，则位或，如按下左键后不放点击右键，button为3
		//IE9以及其他  左键0 中键1 右键2  组合键没有特殊值
		$(this).enableDrag().disableDrag().bind({
			mousedown : function(e){
				if(!$.isLeftClick(e) || this.disabled || /disabled/gi.test(this.className) )
					return false;
				$(this).addClass(down);
			},
			mouseup : function(e){
				if(!$.isLeftClick(e))
					return false;
				$(this).removeClass(down);
			},
			mouseout : function(){$(this).removeClass(down);}
		});
		//保持状态
		//IE下连续高速点击，将导致一部分click事件丢失 ---- 此bug已经通过脚本修复
		keepDownCss && $(this).click(function(){
			$(this).toggleClass(keepDownCss);
		});
	});
};

})(jQuery);