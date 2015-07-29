
/*
* Ajax overlay 1.0
* Author: Simon Ilett @ aplusdesign.com.au
* Descrip: Creates and inserts an ajax loader for ajax calls / timed events 
* Date: 03/08/2011 
*/
function ajaxLoader(el, options) {
    // Becomes this.options
    var defaults = {
        bgColor: '#fff',
        duration: 800,
        opacity: 0.5,
        classOveride: false
    }
    this.options = jQuery.extend(defaults, options);
    this.container = $(el);

    this.init = function () {
        var container = this.container;
        // Delete any other loaders
        this.remove();
        var B = document.body;
        var H = document.documentElement;
        var height;

        if (typeof document.height !== 'undefined') {
            height = document.height // For webkit browsers
        } else {
            height = Math.max(B.scrollHeight, B.offsetHeight, H.clientHeight, H.scrollHeight, H.offsetHeight);
        }
        // Create the overlay 
        var overlay = $('<div></div>').css({
            'background-color': this.options.bgColor,
            'opacity': this.options.opacity,
            'width': container.width(),
            //'height':container.height(),
            'height': height+"px",
            'position': 'absolute',
            'top': '0px',
            'left': '0px',
            'z-index': 999999
        }).addClass('ajax_overlay');
        // add an overiding class name to set new loader style 
        if (this.options.classOveride) {
            overlay.addClass(this.options.classOveride);
        }
        // insert overlay and loader into DOM 
        container.append(
			overlay.append(
				$('<div></div>').addClass('ajax_loader')
			).fadeIn(this.options.duration)
		);
    };

    this.remove = function () {
        var overlay = this.container.children(".ajax_overlay");
        if (overlay.length) {
            overlay.fadeOut(this.options.classOveride, function () {
                overlay.remove();
            });
        }
    }

    this.init();
}
