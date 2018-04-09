/*
 * easyEvents组件
 * 2013-06-06 由梁枫建议，参考backbone语法编写
 * unbind 可选，如果为true，则是卸载
 * conf  配置参数，其中key表示事件绑定信息，value表示函数名或者函数
 * owner 可选，conf的宿主对象，即this，默认window
 key格式   event1,evnet2,event3 selector
 value格式 string or function
 */
(function(window, $){
$.easyEvents = function( unbind, conf, owner, _wrap ){
	var fn, doc = $(_wrap||document), match, eventName, selector, method;
	if( Object.prototype.toString.call(unbind) == "[object Object]" ){
		owner = conf;
		conf = unbind;
		unbind = 0;
	}
	method = unbind ? "undelegate" : "delegate";
	owner = owner || window;
	//遍历配置对象
	for(var key in conf){
		fn = $.isFunction(conf[key]) ? conf[key] : $.isFunction(owner[conf[key]]) ? owner[conf[key]] : null;
		if( fn ){ //有事件才继续绑定
			match = key.match(/^(\S+)\s*(.*)$/);
			eventName = match[1];
			selector = match[2];
			if( eventName && selector ){
				doc[method]( selector, eventName.replace(/\,/g, " "), $.proxy(fn, owner) );
			}
		}
	}
};
$.fn.easyEvents = function(unbind, conf, owner){ $.easyEvents(unbind, conf, owner, this); return this; };
})(window,jQuery);