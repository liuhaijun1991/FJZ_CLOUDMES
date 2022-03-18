/*!
 * jQuery ClassyColor
 * vox.SPACE
 *
 * Written by Marius Stanciu - Sergiu <marius@vox.space>
 * Licensed under the MIT license https://vox.SPACE/LICENSE-MIT
 * Version 1.1.0
 *
 */

(function ($) {
    "use strict";

    function b(a, b, c) {
        return Math.max(Math.min(a, c), b);
    }

    function c(a) {
        return a.toFixed(2);
    }

    function d(a) {
        return (255 * a | 0) + '';
    }

    function e(a) {
        return (360 * a | 0) + '';
    }

    function f(a) {
        return (100 * a).toFixed(2) + '%';
    }

    function g(a, b, c) {
        var d, e = c ? b.split("").map(function (a) {
            return n[a](u[a]);
        }) : b.split(""),
                f = b.indexOf(a),
                g = [],
                h = v[a];
        for (d = 0; h > d; ++d) {
            var i = d / (h - 1);
            e[f] = n[a](i);
            var j = q ? b + "(" + e.join().toUpperCase() + ")" : '<stop stop-color="' + b + "(" + e.join().toUpperCase() + ')" offset="' + i + '"/>';
            g.push(j);
        }
        return g = g.join(q ? ',' : ''),
                function (b) {
                    var c = g;
                    for (var d in b)
                        d === a || (c = c.replace(m[d], n[d](b[d])));
                    if (q)
                        return 'linear-gradient(to right, ' + c + ')';
                    var e = '<svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%" viewBox="0 0 1 1" preserveAspectRatio="none"><linearGradient id="the-gradient" gradientUnits="userSpaceOnUse" x1="0%" y1="0%" x2="100%" y2="0%">' + c + '</linearGradient><rect x="0" y="0" width="1" height="1" fill="url(#the-gradient)" /></svg>';
                    return "url(data:image/svg+xml;base64," + _btoa(e) + ")";
                };
    }

    function h(a) {
        function b(a, b, c) {
            return 0 > c && (c += 1), c > 1 && (c -= 1), 1 / 6 > c ? a + 6 * (b - a) * c : .5 > c ? b : 2 / 3 > c ? a + (b - a) * (2 / 3 - c) * 6 : a;
        }
        if ('r' in a)
            return a;
        var c, d, e, f = a.h,
                g = a.s,
                h = a.l;
        if (0 === g)
            c = d = e = h;
        else {
            var i = .5 > h ? h * (1 + g) : h + g - h * g,
                    j = 2 * h - i;
            c = b(j, i, f + 1 / 3), d = b(j, i, f), e = b(j, i, f - 1 / 3);
        }
        var k = {
            r: c,
            g: d,
            b: e
        };
        return 'a' in a && (k.a = a.a), k;
    }

    function i(a) {
        if ('h' in a)
            return a;
        var b, c;
        var d = a.r,
                e = a.g,
                f = a.b,
                g = Math.max(d, e, f),
                h = Math.min(d, e, f),
                i = (g + h) / 2;
        if (g === h)
            b = c = 0;
        else {
            var j = g - h;
            switch (c = i > .5 ? j / (2 - g - h) : j / (g + h), g) {
                case d:
                    b = (e - f) / j + (f > e ? 6 : 0);
                    break;
                case e:
                    b = (f - d) / j + 2;
                    break;
                case f:
                    b = (d - e) / j + 4;
            }
            b /= 6;
        }
        var k = {
            h: b,
            s: c,
            l: i
        };
        return 'a' in a && (k.a = a.a), k;
    }

    function ClassyColorObj(b, color) {
        var d, e = {};
        if (b = b.toLowerCase(), !/^(rgb|hsl)a?$/i.test(b))
            throw 'Color spaces must be any of the following: "rgb", "rgba", "hsl" or "hsla"';
        if ('string' === typeof color)
            if (color = color.toLowerCase(), /^#[0-9a-f]{6}([0-9a-f]{2})?$/.test(color))
                for (4 === b.length && 9 !== color.length && (color += 'ff'), d = 1; d < color.length; d += 2)
                    e[b[(d - 1) / 2]] = parseInt(color.substr(d, 2), 16) / 255;
            else {
                if (!/^(rgb|hsl)a?\([\d\s,.%]+\)$/.test(color))
                    throw 'Color strings must be hexadecimal (e.g. "#00ff00", or "#00ff00ff") or CSS style (e.g. rgba(0,255,0,1))';
                var f = color.match(/(rgb|hsl)a?/)[0], g = color.match(/[\d.]+/g);
                /rgb/.test(f) ? (e.r = g[0] / 255, e.g = g[1] / 255, e.b = g[2] / 255) : (e.h = g[0] / 360, e.s = g[1] / 100, e.l = g[2] / 100), e.a = +g[3];
            }
        else {
            if (!$.isPlainObject(color))
                throw 'Unrecognized color format.';
            var j = Object.keys(color).sort().join("");
            if (!/^a?(bgr|hls)$/i.test(j))
                throw 'Color objects must contain either r, g and b keys, or h, s and l keys. The a key is optional.';
            b.split('').sort().join('') !== j && (color = /rgb/.test('space') ? h(color) : i(color)), e = color;
        }
        e = /rgb/.test(b) ? h(e) : i(e), isFinite(e.a) || (e.a = 1);
        for (var k in e)
            if (e[k] < 0 || 1 < e[k])
                throw 'Color component out of range: ' + k;
        this.getComponents = function () {
            return e;
        }, this.getComponent = function (a) {
            return e[a];
        }, this.setComponent = function (a, b) {
            e[a] = b;
        }, this.getSpace = function () {
            return b;
        };
    }

    function ClassyColor(elem, options) {
        function addClassAndAppendTo(elmClass, container) {
            return $('<div>').addClass(elmClass).appendTo(container);
        }

        function f(a, b, c, d) {
            c && obj.setComponent(c, d);
            for (var e = 0; e < G.length; ++e) {
                var f = G[e];
                f.css({
                    backgroundImage: f.data('template')(obj.getComponents())
                }).find('.handle').css({
                    left: 100 * obj.getComponent(f.data('componentKey')) + '%'
                }), E.css({
                    backgroundColor: obj.toCssString()
                }), settings.displayColor && o.text(p());
            }
        }

        var h = this;
        var i = elem.attr('class');
        var k = elem.children().detach();
        var l = elem.text();
        elem.addClass('ClassyColor').empty();
        var settings = $.extend({
            color: elem.css('color'),
            colorSpace: 'rgba',
            expandEvent: 'mousedown touchstart',
            collapseEvent: '',
            staticComponents: false
        }, options);
        var obj = new ClassyColorObj(settings.colorSpace, settings.color);
        elem.addClass('componentcount-' + obj.getSpace().length).toggleClass('show-labels', !!settings.labels);
        var o, p;
        var t = addClassAndAppendTo('maximize-wrapper', elem),
                u = addClassAndAppendTo('inner-maximize-wrapper', t),
                v = addClassAndAppendTo('ui-wrapper', u),
                D = addClassAndAppendTo('display-wrapper', v),
                E = addClassAndAppendTo('display', D),
                F = addClassAndAppendTo('slider-container', v);
        var G = $.map(obj.getSpace().split(''), function (a) {
            var b = addClassAndAppendTo('slider ' + a, F);
            return addClassAndAppendTo('handle', b).attr('data-component', a), b.data({
                componentKey: a,
                template: g(a, obj.getSpace(), settings.staticComponents)
            }), b;
        });
        if (settings.displayColor) {
            if (!/^(hex|css)$/.test(settings.displayColor))
                throw 'Invalid displayColor value, should be "hex" or "css"';
            o = addClassAndAppendTo('output-wrapper', u);
            p = {
                hex: obj.toString,
                css: obj.toCssString
            }[settings.displayColor].bind(obj, settings.displayColorSpace || settings.colorSpace);
        }
        elem.on('newcolor', f), setTimeout(f.bind(null, void 0, this), 0);
        var doExpand, doCollapse, show = function (b) {
            $(b.target).closest(elem).length || doCollapse();
        };
        doExpand = function () {
            elem.addClass('expanded').css({
                zIndex: 999999
            });
            var b = 0;
            u.children().each(function () {
                b += $(this).outerHeight(true);
            }), u.css({
                width: v.width(),
                height: b
            }), $(window).on(startEvent, show), settings.collapseEvent && elem.on(settings.collapseEvent, doCollapse), elem.off(settings.expandEvent, doExpand);
            var d = $(document).width(),
                    e = $(document).height(),
                    f = v.offset(),
                    g = Math.min(0, d - f.left - v.outerWidth(true) - 10),
                    h = Math.min(0, e - f.top - v.outerHeight(true) - 10);
            elem.css('transform', 'translate(' + g + 'px, ' + h + 'px)');
        };
        doCollapse = function () {
            r ? elem.off(x).one(x, function () {
                elem.css({
                    zIndex: ''
                });
            }) : elem.css({
                zIndex: ''
            }), elem.css({
                zIndex: 999999 - 1
            }), u.css({
                width: '',
                height: ''
            }), elem.removeClass('expanded'), $(window).off(startEvent, show), settings.expandEvent && elem.on(settings.expandEvent, doExpand), elem.css('transform', '');
        }, settings.expandEvent && elem.on(settings.expandEvent, doExpand);
        var onMove = function (c) {
            var d = $(c.target);
            if (d.hasClass("slider")) {
                var e = hasTouchEvent ? c.originalEvent.touches[0].clientX - $(c.target).offset().left : $.isNumeric(c.offsetX) ? c.offsetX : c.clientX - $(c.target).offset().left;
                d.trigger('newcolor', [h, d.data("componentKey"), b(e / d.outerWidth(), 0, 1)]), c.preventDefault(), c.stopPropagation();
            }
        };
        var onEnd = function () {
            $(window).off(moveEvent, onMove);
        };
        this.destroy = function () {
            $(window).off(endEvent, onEnd);
            $(window).off(moveEvent, onMove);
            $(window).off(startEvent, show);
            elem.off().empty().removeData('ClassyColor').attr('class', i).html(k).text(l);
        };
        this.toString = function () {
            return obj.toString.apply(obj, arguments);
        };
        this.toCssString = function () {
            return obj.toCssString.apply(obj, arguments);
        };
        this.toObject = function () {
            return obj.convertComponents.apply(obj, arguments);
        };
        this.getColorSpace = function () {
            return obj.getSpace();
        };
        this.on = elem.on.bind(elem);
        this.off = elem.off.bind(elem);
        elem.data('ClassyColor', this);
        return elem.on(startEvent, function (elm) {
            'mousedown' === elm.type && elm.button !== w || !$(elm.target).hasClass('slider') || (onMove(elm), $(window).on(moveEvent, onMove), $(window).one(endEvent, onEnd));
        });
    }
    var _btoa = window.btoa || function (a) {
        for (var b, c, d, e = [], f = -1, g = a.length, h = [0, 0, 0, 0]; ++f < g; ) {
            b = a.charCodeAt(f);
            c = a.charCodeAt(++f);
            h[0] = b >> 2;
            h[1] = (3 & b) << 4 | c >> 4;
            isNaN(c) ? h[2] = h[3] = 64 : (d = a.charCodeAt(++f), h[2] = (15 & c) << 2 | d >> 6, h[3] = isNaN(d) ? 64 : 63 & d);
            e.push(t.charAt(h[0]), t.charAt(h[1]), t.charAt(h[2]), t.charAt(h[3]));
        }
        return e.join('');
    };
    var m = {
        r: /R/g,
        g: /G/g,
        b: /B/g,
        h: /H/g,
        s: /S/g,
        l: /L/g,
        a: /A/g
    };
    var n = {
        r: d,
        g: d,
        b: d,
        h: e,
        s: f,
        l: f,
        a: c
    };
    var isSafari = /Safari/.test(navigator.userAgent) && !/Chrome/.test(navigator.userAgent);
    var isOldIE = /MSIE [1-8]/.test(navigator.userAgent);
    var q = !/MSIE 9.0/.test(navigator.userAgent);
    var r = q;
    var hasTouchEvent = 'ontouchstart' in document.documentElement;
    var t = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';
    var u = {
        r: 0,
        g: 0,
        b: 0,
        h: 0,
        s: 1,
        l: .5,
        a: 1
    };
    var v = {
        r: 2,
        g: 2,
        b: 2,
        h: 7,
        s: 2,
        l: 3,
        a: 2
    };
    var w = 0;
    var x = isSafari ? 'webkitTransitionEnd' : 'transitionend';
    var startEvent = hasTouchEvent ? 'touchstart' : 'mousedown';
    var moveEvent = hasTouchEvent ? 'touchmove' : 'mousemove';
    var endEvent = hasTouchEvent ? 'touchend touchcancel' : 'mouseup';

    ClassyColorObj.prototype.convertComponents = function (b) {
        b = b || this.getSpace();
        var c = $.extend({}, this.getComponents());
        if (new RegExp(b).test(this.getSpace()))
            return 3 === b.length && delete c.a, c;
        if (c = /rgb/.test(b) ? h(c) : i(c), /a/.test(b)) {
            var d = this.getComponent('a');
            c.a = 'undefined' == typeof d ? 1 : d;
        }
        return c;
    };

    ClassyColorObj.prototype.componentsToString = function (a, b) {
        function c(a) {
            return (1 === a.length ? '0' : '') + a;
        }
        for (var d = '#', e = 0; e < b.length; ++e)
            d += c(Math.floor(255 * a[b[e]]).toString(16));
        return d;
    };

    ClassyColorObj.prototype.toString = function (data) {
        return data = data || this.getSpace(), this.componentsToString(this.convertComponents(data), data);
    };

    ClassyColorObj.prototype.componentsToCssValuesString = function (a, b) {
        for (var c = [], d = 0; d < b.length; ++d) {
            var e = b[d];
            c.push(n[e](a[e]));
        }
        return c.join(', ');
    };

    ClassyColorObj.prototype.toCssString = function (data) {
        return data = data || this.getSpace(), data + '(' + this.componentsToCssValuesString(this.convertComponents(data), data) + ')';
    };

    $.fn.ClassyColor = function (options) {
        if (isOldIE)
            throw 'Unfortunately, ClassyColor does not work on your browser.';
        var container = this.eq(0);
        if (!container.length)
            throw "No element matched";
        return container.data('ClassyColor') || new ClassyColor(container, options);
    };
})(jQuery);