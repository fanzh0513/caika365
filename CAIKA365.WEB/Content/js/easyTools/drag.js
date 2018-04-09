/*
 * 简易拖拉组件 $.fn.bindDrag
 * 不使用浏览器自带的拖拉事件，而是使用mousedown / mousemove / mouseup 事件
 * 一个组合参数，事件函数的调用者都指向Dom元素本身
 * {
 *	beforeDrag : fn,//鼠标按下时触发，接收一个参数，event 对象，若fn返回flase则不拖动
 *	dragStart : fn,	//准备拖动前触发，接收一个参数，event 对象，若fn返回flase则不拖动
 *	onDrag : fn,	//拖动中不断触发，接收一个参数，event 对象
 *	dragEnd : fn,	//拖动结束时触发，接收一个参数，event 对象
 *	pix : 3			//启用拖动像素差，不得小于1，不得大于10
 * }
 */
(function($){
$.fn.bindDrag = function( options ){
	var op = $.extend({
			  beforeDrag:$.noop,
			  dragStart:$.noop,
			  onDrag:$.noop,
			  dragEnd:$.noop,
			  pix : 3
			}, options||{} ),
		dragCache,
		dragEvents = {
			mousedown : function(e){
				if( op.beforeDrag.call(this, e) === false) //用户停止
					return; //由原来的return false 修改 return; 2012-03-02 马超
				//缓存鼠标位置并标记
				dragCache = {
					mouse : [e.pageX, e.pageY],
					flag : 1
				};
				this.setCapture
					? this.setCapture()
					: window.captureEvents && window.captureEvents(window.Event.MOUSEMOVE|window.Event.MOUSEUP);
				$(this).one("losecapture", function(){$(document).mouseup()});
				//绑定document进行监听
				$(document).mousemove($.proxy(dragEvents.mousemove, this))
					.mouseup($.proxy(dragEvents.mouseup, this));
				//仅阻止默认行为，不阻止冒泡
				e.preventDefault();
			},
			mousemove : function(e){
				var cache = dragCache;
				if( cache.flag < 1 )
					return;
				if( cache.flag > 1 ){
					op.onDrag.call(this, e);
				}else if( Math.abs(e.pageX-cache.mouse[0])>=op.pix || Math.abs(e.pageY-cache.mouse[1])>=op.pix ){
					cache.flag = 2;
					if( op.dragStart.call(this, e) === false){ //用户停止
						cache.flag = 1;
						dragEvents.mouseup.call(this, e);
					}
				}
			},
			mouseup : function(e){
				var cache = dragCache;
				if(cache.flag > 1)
					op.dragEnd.call(this, e);
				//重置标签
				cache.flag = 0;
				this.releaseCapture
					? this.releaseCapture()
					: window.releaseEvents && window.releaseEvents(window.Event.MOUSEMOVE|window.Event.MOUSEUP);
				$(this).unbind("losecapture");
				//取消事件监听
				$(document).unbind("mousemove", dragEvents.mousemove)
					.unbind("mouseup", dragEvents.mouseup);
				return false;
			}
		};
	//像素差范围 [1,9]
	op.pix = op.pix < 1 ? 1 : op.pix > 9 ? 9 : op.pix;
	//绑定mousedown监听触发
	return this.mousedown(dragEvents.mousedown);
};
})(jQuery);