
 //------  document instruct to use-------------

 // 1. include these libraries to your page 
 //   <script src="~/Scripts/jquery-1.10.2.min.js"></script>
 //   <script src="~/Scripts/xlsx.core.min.js"></script>
 //   <script src="~/Scripts/plugins/bootstrap/bootstrap.min.js"></script>
 //   <script src="~/Scripts/bootstrap-table.min.js"></script>
 // 2. Example using:
 //   <table  class="boostrap_table_class_name">     -- "boostrap_table_class_name": name of table(a boostrap table) which you want to get it's data to export to excel file 
 //    </table>
 //    <div  class="temp_container" style="display:none;">   -- a container include temp data of below function
 //    </div>
 //    <button class="btn btn-warning" onclick="export_excel('boostrap_table_class_name','temp_container');">  Export</button>   --button fire call to  function export_excel()





////////////////////////////////////////////////////////////////////////////////////////

//FileSave.min.js
var saveAs = saveAs || function (e) { "use strict"; if (typeof e === "undefined" || typeof navigator !== "undefined" && /MSIE [1-9]\./.test(navigator.userAgent)) { return } var t = e.document, n = function () { return e.URL || e.webkitURL || e }, r = t.createElementNS("http://www.w3.org/1999/xhtml", "a"), o = "download" in r, a = function (e) { var t = new MouseEvent("click"); e.dispatchEvent(t) }, i = /constructor/i.test(e.HTMLElement) || e.safari, f = /CriOS\/[\d]+/.test(navigator.userAgent), u = function (t) { (e.setImmediate || e.setTimeout)(function () { throw t }, 0) }, s = "application/octet-stream", d = 1e3 * 40, c = function (e) { var t = function () { if (typeof e === "string") { n().revokeObjectURL(e) } else { e.remove() } }; setTimeout(t, d) }, l = function (e, t, n) { t = [].concat(t); var r = t.length; while (r--) { var o = e["on" + t[r]]; if (typeof o === "function") { try { o.call(e, n || e) } catch (a) { u(a) } } } }, p = function (e) { if (/^\s*(?:text\/\S*|application\/xml|\S*\/\S*\+xml)\s*;.*charset\s*=\s*utf-8/i.test(e.type)) { return new Blob([String.fromCharCode(65279), e], { type: e.type }) } return e }, v = function (t, u, d) { if (!d) { t = p(t) } var v = this, w = t.type, m = w === s, y, h = function () { l(v, "writestart progress write writeend".split(" ")) }, S = function () { if ((f || m && i) && e.FileReader) { var r = new FileReader; r.onloadend = function () { var t = f ? r.result : r.result.replace(/^data:[^;]*;/, "data:attachment/file;"); var n = e.open(t, "_blank"); if (!n) e.location.href = t; t = undefined; v.readyState = v.DONE; h() }; r.readAsDataURL(t); v.readyState = v.INIT; return } if (!y) { y = n().createObjectURL(t) } if (m) { e.location.href = y } else { var o = e.open(y, "_blank"); if (!o) { e.location.href = y } } v.readyState = v.DONE; h(); c(y) }; v.readyState = v.INIT; if (o) { y = n().createObjectURL(t); setTimeout(function () { r.href = y; r.download = u; a(r); h(); c(y); v.readyState = v.DONE }); return } S() }, w = v.prototype, m = function (e, t, n) { return new v(e, t || e.name || "download", n) }; if (typeof navigator !== "undefined" && navigator.msSaveOrOpenBlob) { return function (e, t, n) { t = t || e.name || "download"; if (!n) { e = p(e) } return navigator.msSaveOrOpenBlob(e, t) } } w.abort = function () { }; w.readyState = w.INIT = 0; w.WRITING = 1; w.DONE = 2; w.error = w.onwritestart = w.onprogress = w.onwrite = w.onabort = w.onerror = w.onwriteend = null; return m }(typeof self !== "undefined" && self || typeof window !== "undefined" && window || this.content); if (typeof module !== "undefined" && module.exports) { module.exports.saveAs = saveAs } else if (typeof define !== "undefined" && define !== null && define.amd !== null) { define("FileSaver.js", function () { return saveAs }) }


/*
 tableExport.jquery.plugin
 Version 1.9.8
 Copyright (c) 2015-2017 hhurz, https://github.com/hhurz
 Original Work Copyright (c) 2014 Giri Raj
 Licensed under the MIT License
*/
var $jscomp = $jscomp || {}; $jscomp.scope = {}; $jscomp.findInternal = function (c, f, u) { c instanceof String && (c = String(c)); for (var C = c.length, D = 0; D < C; D++) { var O = c[D]; if (f.call(u, O, D, c)) return { i: D, v: O } } return { i: -1, v: void 0 } }; $jscomp.ASSUME_ES5 = !1; $jscomp.ASSUME_NO_NATIVE_MAP = !1; $jscomp.ASSUME_NO_NATIVE_SET = !1; $jscomp.defineProperty = $jscomp.ASSUME_ES5 || "function" == typeof Object.defineProperties ? Object.defineProperty : function (c, f, u) { c != Array.prototype && c != Object.prototype && (c[f] = u.value) };
$jscomp.getGlobal = function (c) { return "undefined" != typeof window && window === c ? c : "undefined" != typeof global && null != global ? global : c }; $jscomp.global = $jscomp.getGlobal(this); $jscomp.polyfill = function (c, f, u, C) { if (f) { u = $jscomp.global; c = c.split("."); for (C = 0; C < c.length - 1; C++) { var D = c[C]; D in u || (u[D] = {}); u = u[D] } c = c[c.length - 1]; C = u[c]; f = f(C); f != C && null != f && $jscomp.defineProperty(u, c, { configurable: !0, writable: !0, value: f }) } };
$jscomp.polyfill("Array.prototype.find", function (c) { return c ? c : function (c, u) { return $jscomp.findInternal(this, c, u).v } }, "es6", "es3");
(function (c) {
    c.fn.extend({
        tableExport: function (f) {
            function u(b) { var e = []; C(b, "tbody").each(function () { e.push.apply(e, D(c(this), a.tbodySelector)) }); a.tfootSelector.length && C(b, "tfoot").each(function () { e.push.apply(e, D(c(this), a.tfootSelector)) }); return e } function C(b, e) { var a = b.parents("table").length; return b.find(e).filter(function () { return c(this).closest("table").parents("table").length === a }) } function D(b, e) { return b.find(e).filter(function () { return 0 === c(this).find("table").length && 1 === c(this).parents("table").length }) }
            function O(b) { var e = []; c(b).find("thead").first().find("th").each(function (b, a) { void 0 !== c(a).attr("data-field") ? e[b] = c(a).attr("data-field") : e[b] = b.toString() }); return e } function P(b) {
                var e = "undefined" !== typeof b[0].cellIndex, a = "undefined" !== typeof b[0].rowIndex, r = e || a ? ya(b) : b.is(":visible"), g = b.data("tableexport-display"); e && "none" != g && "always" != g && (b = c(b[0].parentNode), a = "undefined" !== typeof b[0].rowIndex, g = b.data("tableexport-display")); a && "none" != g && "always" != g && (g = b.closest("table").data("tableexport-display"));
                return "none" !== g && (1 == r || "always" == g)
            } function ya(b) { var e = []; R && (e = K.filter(function () { var e = !1; this.nodeType == b[0].nodeType && ("undefined" !== typeof this.rowIndex && this.rowIndex == b[0].rowIndex ? e = !0 : "undefined" !== typeof this.cellIndex && this.cellIndex == b[0].cellIndex && "undefined" !== typeof this.parentNode.rowIndex && "undefined" !== typeof b[0].parentNode.rowIndex && this.parentNode.rowIndex == b[0].parentNode.rowIndex && (e = !0)); return e })); return 0 == R || 0 == e.length } function za(b, e, k) {
                var r = !1; P(b) ? 0 < a.ignoreColumn.length &&
                (-1 != c.inArray(k, a.ignoreColumn) || -1 != c.inArray(k - e, a.ignoreColumn) || Q.length > k && "undefined" != typeof Q[k] && -1 != c.inArray(Q[k], a.ignoreColumn)) && (r = !0) : r = !0; return r
            } function B(b, e, k, r, g) {
                if ("function" === typeof g) {
                    var h = !1; "function" === typeof a.onIgnoreRow && (h = a.onIgnoreRow(c(b), k)); if (!1 === h && -1 == c.inArray(k, a.ignoreRow) && -1 == c.inArray(k - r, a.ignoreRow) && P(c(b))) {
                        var x = c(b).find(e), q = 0; x.each(function (b) {
                            var e = c(this), a, h = parseInt(this.getAttribute("colspan")), r = parseInt(this.getAttribute("rowspan"));
                            G.forEach(function (b) { if (k >= b.s.r && k <= b.e.r && q >= b.s.c && q <= b.e.c) for (a = 0; a <= b.e.c - b.s.c; ++a) g(null, k, q++) }); if (!1 === za(e, x.length, b)) { if (r || h) h = h || 1, G.push({ s: { r: k, c: q }, e: { r: k + (r || 1) - 1, c: q + h - 1 } }); g(this, k, q++) } if (h) for (a = 0; a < h - 1; ++a) g(null, k, q++)
                        }); G.forEach(function (b) { if (k >= b.s.r && k <= b.e.r && q >= b.s.c && q <= b.e.c) for (Y = 0; Y <= b.e.c - b.s.c; ++Y) g(null, k, q++) })
                    }
                }
            } function la(b, e) {
                !0 === a.consoleLog && console.log(b.output()); if ("string" === a.outputMode) return b.output(); if ("base64" === a.outputMode) return L(b.output());
                if ("window" === a.outputMode) window.URL = window.URL || window.webkitURL, window.open(window.URL.createObjectURL(b.output("blob"))); else try { var k = b.output("blob"); saveAs(k, a.fileName + ".pdf") } catch (r) { H(a.fileName + ".pdf", "data:application/pdf" + (e ? "" : ";base64") + ",", e ? b.output("blob") : b.output()) }
            } function ma(b, e, a) {
                var k = 0; "undefined" !== typeof a && (k = a.colspan); if (0 <= k) {
                    for (var g = b.width, c = b.textPos.x, x = e.table.columns.indexOf(e.column), q = 1; q < k; q++) g += e.table.columns[x + q].width; 1 < k && ("right" === b.styles.halign ?
                    c = b.textPos.x + g - b.width : "center" === b.styles.halign && (c = b.textPos.x + (g - b.width) / 2)); b.width = g; b.textPos.x = c; "undefined" !== typeof a && 1 < a.rowspan && (b.height *= a.rowspan); if ("middle" === b.styles.valign || "bottom" === b.styles.valign) a = ("string" === typeof b.text ? b.text.split(/\r\n|\r|\n/g) : b.text).length || 1, 2 < a && (b.textPos.y -= (2 - 1.15) / 2 * e.row.styles.fontSize * (a - 2) / 3); return !0
                } return !1
            } function na(b, a, k) {
                "undefined" != typeof k.images && a.each(function () {
                    var a = c(this).children(); if (c(this).is("img")) {
                        var e = oa(this.src);
                        k.images[e] = { url: this.src, src: this.src }
                    } "undefined" != typeof a && 0 < a.length && na(b, a, k)
                })
            } function Aa(b, a) {
                function e(b) { if (b.url) { var e = new Image; g = ++h; e.crossOrigin = "Anonymous"; e.onerror = e.onload = function () { if (e.complete && (0 === e.src.indexOf("data:image/") && (e.width = b.width || e.width || 0, e.height = b.height || e.height || 0), e.width + e.height)) { var k = document.createElement("canvas"), c = k.getContext("2d"); k.width = e.width; k.height = e.height; c.drawImage(e, 0, 0); b.src = k.toDataURL("image/jpeg") } --h || a(g) }; e.src = b.url } }
                var c, g = 0, h = 0; if ("undefined" != typeof b.images) for (c in b.images) b.images.hasOwnProperty(c) && e(b.images[c]); (b = h) || (a(g), b = void 0); return b
            } function pa(b, e, k) {
                e.each(function () {
                    var e = c(this).children(), g = 0; if (c(this).is("div")) {
                        var h = Z(M(this, "background-color"), [255, 255, 255]), x = Z(M(this, "border-top-color"), [0, 0, 0]), q = aa(this, "border-top-width", a.jspdf.unit), d = this.getBoundingClientRect(), f = this.offsetLeft * k.dw; g = this.offsetTop * k.dh; var l = d.width * k.dw; d = d.height * k.dh; k.doc.setDrawColor.apply(void 0,
                        x); k.doc.setFillColor.apply(void 0, h); k.doc.setLineWidth(q); k.doc.rect(b.x + f, b.y + g, l, d, q ? "FD" : "F")
                    } else if (c(this).is("img") && "undefined" != typeof k.images && (h = oa(this.src), h = k.images[h], "undefined" != typeof h)) {
                        x = b.width / b.height; q = this.width / this.height; f = b.width; l = b.height; d = 19.049976 / 25.4; q <= x ? (l = Math.min(b.height, this.height), f = this.width * l / this.height) : q > x && (f = Math.min(b.width, this.width), l = this.height * f / this.width); f *= d; l *= d; l < b.height && (g = (b.height - l) / 2); try {
                            k.doc.addImage(h.src, b.textPos.x,
                            b.y + g, f, l)
                        } catch (Ea) { } b.textPos.x += f
                    } "undefined" != typeof e && 0 < e.length && pa(b, e, k)
                })
            } function qa(b, e, a) {
                if ("function" === typeof a.onAutotableText) a.onAutotableText(a.doc, b, e); else {
                    var k = b.textPos.x, g = b.textPos.y, h = { halign: b.styles.halign, valign: b.styles.valign }; if (e.length) {
                        for (e = e[0]; e.previousSibling;) e = e.previousSibling; for (var x = !1, q = !1; e;) {
                            var d = e.innerText || e.textContent || ""; d = (d.length && " " == d[0] ? " " : "") + c.trim(d) + (1 < d.length && " " == d[d.length - 1] ? " " : ""); c(e).is("br") && (k = b.textPos.x, g += a.doc.internal.getFontSize());
                            c(e).is("b") ? x = !0 : c(e).is("i") && (q = !0); (x || q) && a.doc.setFontType(x && q ? "bolditalic" : x ? "bold" : "italic"); var f = a.doc.getStringUnitWidth(d) * a.doc.internal.getFontSize(); if (f) {
                                if ("linebreak" === b.styles.overflow && k > b.textPos.x && k + f > b.textPos.x + b.width) {
                                    if (0 <= ".,!%*;:=-".indexOf(d.charAt(0))) { var l = d.charAt(0); f = a.doc.getStringUnitWidth(l) * a.doc.internal.getFontSize(); k + f <= b.textPos.x + b.width && (a.doc.autoTableText(l, k, g, h), d = d.substring(1, d.length)); f = a.doc.getStringUnitWidth(d) * a.doc.internal.getFontSize() } k =
                                    b.textPos.x; g += a.doc.internal.getFontSize()
                                } for (; d.length && k + f > b.textPos.x + b.width;) d = d.substring(0, d.length - 1), f = a.doc.getStringUnitWidth(d) * a.doc.internal.getFontSize(); a.doc.autoTableText(d, k, g, h); k += f
                            } if (x || q) c(e).is("b") ? x = !1 : c(e).is("i") && (q = !1), a.doc.setFontType(x || q ? x ? "bold" : "italic" : "normal"); e = e.nextSibling
                        } b.textPos.x = k; b.textPos.y = g
                    } else a.doc.autoTableText(b.text, b.textPos.x, b.textPos.y, h)
                }
            } function ba(b, a, c) {
                return b.replace(new RegExp(a.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1"),
                "g"), c)
            } function ea(b) { b = ba(b || "0", a.numbers.html.thousandsSeparator, ""); b = ba(b, a.numbers.html.decimalMark, "."); return "number" === typeof b || !1 !== jQuery.isNumeric(b) ? b : !1 } function Ba(b) { -1 < b.indexOf("%") ? (b = ea(b.replace(/%/g, "")), !1 !== b && (b /= 100)) : b = !1; return b } function z(b, e, k) {
                var r = ""; if (null !== b) {
                    var g = c(b); if (g[0].hasAttribute("data-tableexport-value")) var h = (h = g.data("tableexport-value")) ? h + "" : ""; else if (h = g.html(), "function" === typeof a.onCellHtmlData) h = a.onCellHtmlData(g, e, k, h); else if ("" != h) {
                        var d =
                        c.parseHTML(h), f = 0, l = 0; h = ""; c.each(d, function () { if (c(this).is("input")) h += g.find("input").eq(f++).val(); else if (c(this).is("select")) h += g.find("select option:selected").eq(l++).text(); else if ("undefined" === typeof c(this).html()) h += c(this).text(); else if (void 0 === jQuery().bootstrapTable || !0 !== c(this).hasClass("filterControl") && 0 === c(b).parents(".detail-view").length) h += c(this).html() })
                    } if (!0 === a.htmlContent) r = c.trim(h); else if (h && "" != h) if ("" != c(b).data("tableexport-cellformat")) {
                        var n = h.replace(/\n/g,
                        "\u2028").replace(/<br\s*[\/]?>/gi, "\u2060"), m = c("<div/>").html(n).contents(); d = !1; n = ""; c.each(m.text().split("\u2028"), function (b, a) { 0 < b && (n += " "); n += c.trim(a) }); c.each(n.split("\u2060"), function (b, a) { 0 < b && (r += "\n"); r += c.trim(a).replace(/\u00AD/g, "") }); if ("json" == a.type || "excel" === a.type && "xmlss" === a.excelFileFormat || !1 === a.numbers.output) d = ea(r), !1 !== d && (r = Number(d)); else if (a.numbers.html.decimalMark != a.numbers.output.decimalMark || a.numbers.html.thousandsSeparator != a.numbers.output.thousandsSeparator) if (d =
                        ea(r), !1 !== d) { m = ("" + d.substr(0 > d ? 1 : 0)).split("."); 1 == m.length && (m[1] = ""); var p = 3 < m[0].length ? m[0].length % 3 : 0; r = (0 > d ? "-" : "") + (a.numbers.output.thousandsSeparator ? (p ? m[0].substr(0, p) + a.numbers.output.thousandsSeparator : "") + m[0].substr(p).replace(/(\d{3})(?=\d)/g, "$1" + a.numbers.output.thousandsSeparator) : m[0]) + (m[1].length ? a.numbers.output.decimalMark + m[1] : "") }
                    } else r = h; !0 === a.escape && (r = escape(r)); "function" === typeof a.onCellData && (r = a.onCellData(g, e, k, r))
                } return r
            } function Ca(b, a, c) { return a + "-" + c.toLowerCase() }
            function Z(b, a) { (b = /^rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$/.exec(b)) && (a = [parseInt(b[1]), parseInt(b[2]), parseInt(b[3])]); return a } function ra(b) {
                var a = M(b, "text-align"), k = M(b, "font-weight"), r = M(b, "font-style"), g = ""; "start" == a && (a = "rtl" == M(b, "direction") ? "right" : "left"); 700 <= k && (g = "bold"); "italic" == r && (g += r); "" === g && (g = "normal"); a = {
                    style: { align: a, bcolor: Z(M(b, "background-color"), [255, 255, 255]), color: Z(M(b, "color"), [0, 0, 0]), fstyle: g }, colspan: parseInt(c(b).attr("colspan")) || 0, rowspan: parseInt(c(b).attr("rowspan")) ||
                    0
                }; null !== b && (b = b.getBoundingClientRect(), a.rect = { width: b.width, height: b.height }); return a
            } function M(b, a) { try { return window.getComputedStyle ? (a = a.replace(/([a-z])([A-Z])/, Ca), window.getComputedStyle(b, null).getPropertyValue(a)) : b.currentStyle ? b.currentStyle[a] : b.style[a] } catch (k) { } return "" } function aa(b, a, c) {
                a = M(b, a).match(/\d+/); if (null !== a) {
                    a = a[0]; b = b.parentElement; var e = document.createElement("div"); e.style.overflow = "hidden"; e.style.visibility = "hidden"; b.appendChild(e); e.style.width = 100 + c; c = 100 /
                    e.offsetWidth; b.removeChild(e); return a * c
                } return 0
            } function fa() { if (!(this instanceof fa)) return new fa; this.SheetNames = []; this.Sheets = {} } function sa(b) { for (var a = new ArrayBuffer(b.length), c = new Uint8Array(a), d = 0; d != b.length; ++d) c[d] = b.charCodeAt(d) & 255; return a } function Da(b) {
                for (var a = {}, c = { s: { c: 1E7, r: 1E7 }, e: { c: 0, r: 0 } }, d = 0; d != b.length; ++d) for (var g = 0; g != b[d].length; ++g) {
                    c.s.r > d && (c.s.r = d); c.s.c > g && (c.s.c = g); c.e.r < d && (c.e.r = d); c.e.c < g && (c.e.c = g); var h = { v: b[d][g] }; if (null !== h.v) {
                        var f = XLSX.utils.encode_cell({
                            c: g,
                            r: d
                        }); if ("number" === typeof h.v) h.t = "n"; else if ("boolean" === typeof h.v) h.t = "b"; else if (h.v instanceof Date) { h.t = "n"; h.z = XLSX.SSF._table[14]; var q = h; var l = (Date.parse(h.v) - new Date(Date.UTC(1899, 11, 30))) / 864E5; q.v = l } else h.t = "s"; a[f] = h
                    }
                } 1E7 > c.s.c && (a["!ref"] = XLSX.utils.encode_range(c)); return a
            } function oa(b) { var a = 0, c; if (0 === b.length) return a; var d = 0; for (c = b.length; d < c; d++) { var g = b.charCodeAt(d); a = (a << 5) - a + g; a |= 0 } return a } function H(b, a, c) {
                var e = window.navigator.userAgent; if (!1 !== b && window.navigator.msSaveOrOpenBlob) window.navigator.msSaveOrOpenBlob(new Blob([c]),
                b); else if (!1 !== b && (0 < e.indexOf("MSIE ") || e.match(/Trident.*rv\:11\./))) { if (a = document.createElement("iframe")) document.body.appendChild(a), a.setAttribute("style", "display:none"), a.contentDocument.open("txt/html", "replace"), a.contentDocument.write(c), a.contentDocument.close(), a.focus(), a.contentDocument.execCommand("SaveAs", !0, b), document.body.removeChild(a) } else {
                    var g = document.createElement("a"); if (g) {
                        var h = null; g.style.display = "none"; !1 !== b ? g.download = b : g.target = "_blank"; "object" == typeof c ? (window.URL =
                        window.URL || window.webkitURL, h = window.URL.createObjectURL(c), g.href = h) : 0 <= a.toLowerCase().indexOf("base64,") ? g.href = a + L(c) : g.href = a + encodeURIComponent(c); document.body.appendChild(g); if (document.createEvent) null === ca && (ca = document.createEvent("MouseEvents")), ca.initEvent("click", !0, !1), g.dispatchEvent(ca); else if (document.createEventObject) g.fireEvent("onclick"); else if ("function" == typeof g.onclick) g.onclick(); setTimeout(function () { h && window.URL.revokeObjectURL(h); document.body.removeChild(g) }, 100)
                    }
                }
            }
            function L(a) {
                var b, c = "", d = 0; if ("string" === typeof a) { a = a.replace(/\x0d\x0a/g, "\n"); var g = ""; for (b = 0; b < a.length; b++) { var h = a.charCodeAt(b); 128 > h ? g += String.fromCharCode(h) : (127 < h && 2048 > h ? g += String.fromCharCode(h >> 6 | 192) : (g += String.fromCharCode(h >> 12 | 224), g += String.fromCharCode(h >> 6 & 63 | 128)), g += String.fromCharCode(h & 63 | 128)) } a = g } for (; d < a.length;) {
                    var f = a.charCodeAt(d++); g = a.charCodeAt(d++); b = a.charCodeAt(d++); h = f >> 2; f = (f & 3) << 4 | g >> 4; var q = (g & 15) << 2 | b >> 6; var l = b & 63; isNaN(g) ? q = l = 64 : isNaN(b) && (l = 64); c = c +
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".charAt(h) + "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".charAt(f) + "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".charAt(q) + "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=".charAt(l)
                } return c
            } var a = {
                consoleLog: !1, csvEnclosure: '"', csvSeparator: ",", csvUseBOM: !0, displayTableName: !1, escape: !1, excelFileFormat: "xlshtml", excelRTL: !1, excelstyles: [], exportHiddenCells: !1, fileName: "tableExport",
                htmlContent: !1, ignoreColumn: [], ignoreRow: [], jsonScope: "all", jspdf: {
                    orientation: "p", unit: "pt", format: "a4", margins: { left: 20, right: 10, top: 10, bottom: 10 }, onDocCreated: null, autotable: {
                        styles: { cellPadding: 2, rowHeight: 12, fontSize: 8, fillColor: 255, textColor: 50, fontStyle: "normal", overflow: "ellipsize", halign: "left", valign: "middle" }, headerStyles: { fillColor: [52, 73, 94], textColor: 255, fontStyle: "bold", halign: "center" }, alternateRowStyles: { fillColor: 245 }, tableExport: {
                            doc: null, onAfterAutotable: null, onBeforeAutotable: null,
                            onAutotableText: null, onTable: null, outputImages: !0
                        }
                    }
                }, numbers: { html: { decimalMark: ".", thousandsSeparator: "," }, output: { decimalMark: ".", thousandsSeparator: "," } }, onCellData: null, onCellHtmlData: null, onIgnoreRow: null, onMsoNumberFormat: null, outputMode: "file", pdfmake: { enabled: !1, docDefinition: { pageOrientation: "portrait", defaultStyle: { font: "Roboto" } }, fonts: {} }, tbodySelector: "tr", tfootSelector: "tr", theadSelector: "tr", tableName: "Table", type: "csv", worksheetName: ""
            }, v = this, ca = null, p = [], t = [], l = 0, m = "", Q = [], G = [],
            K = [], R = !1; c.extend(!0, a, f); Q = O(v); if ("csv" == a.type || "tsv" == a.type || "txt" == a.type) {
                var I = "", U = 0; G = []; l = 0; var ha = function (b, e, k) {
                    b.each(function () {
                        m = ""; B(this, e, l, k + b.length, function (b, c, e) {
                            var g = m, h = ""; if (null !== b) if (b = z(b, c, e), c = null === b || "" === b ? "" : b.toString(), "tsv" == a.type) b instanceof Date && b.toLocaleString(), h = ba(c, "\t", " "); else if (b instanceof Date) h = a.csvEnclosure + b.toLocaleString() + a.csvEnclosure; else if (h = ba(c, a.csvEnclosure, a.csvEnclosure + a.csvEnclosure), 0 <= h.indexOf(a.csvSeparator) || /[\r\n ]/g.test(h)) h =
                            a.csvEnclosure + h + a.csvEnclosure; m = g + (h + ("tsv" == a.type ? "\t" : a.csvSeparator))
                        }); m = c.trim(m).substring(0, m.length - 1); 0 < m.length && (0 < I.length && (I += "\n"), I += m); l++
                    }); return b.length
                }; U += ha(c(v).find("thead").first().find(a.theadSelector), "th,td", U); C(c(v), "tbody").each(function () { U += ha(D(c(this), a.tbodySelector), "td,th", U) }); a.tfootSelector.length && ha(c(v).find("tfoot").first().find(a.tfootSelector), "td,th", U); I += "\n"; !0 === a.consoleLog && console.log(I); if ("string" === a.outputMode) return I; if ("base64" ===
                a.outputMode) return L(I); if ("window" === a.outputMode) { H(!1, "data:text/" + ("csv" == a.type ? "csv" : "plain") + ";charset=utf-8,", I); return } try { var A = new Blob([I], { type: "text/" + ("csv" == a.type ? "csv" : "plain") + ";charset=utf-8" }); saveAs(A, a.fileName + "." + a.type, "csv" != a.type || !1 === a.csvUseBOM) } catch (b) { H(a.fileName + "." + a.type, "data:text/" + ("csv" == a.type ? "csv" : "plain") + ";charset=utf-8," + ("csv" == a.type && a.csvUseBOM ? "\ufeff" : ""), I) }
            } else if ("sql" == a.type) {
                l = 0; G = []; var w = "INSERT INTO `" + a.tableName + "` ("; p = c(v).find("thead").first().find(a.theadSelector);
                p.each(function () { B(this, "th,td", l, p.length, function (a, c, k) { w += "'" + z(a, c, k) + "'," }); l++; w = c.trim(w); w = c.trim(w).substring(0, w.length - 1) }); w += ") VALUES "; t = u(c(v)); c(t).each(function () { m = ""; B(this, "td,th", l, p.length + t.length, function (a, c, k) { m += "'" + z(a, c, k) + "'," }); 3 < m.length && (w += "(" + m, w = c.trim(w).substring(0, w.length - 1), w += "),"); l++ }); w = c.trim(w).substring(0, w.length - 1); w += ";"; !0 === a.consoleLog && console.log(w); if ("string" === a.outputMode) return w; if ("base64" === a.outputMode) return L(w); try {
                    A = new Blob([w],
                    { type: "text/plain;charset=utf-8" }), saveAs(A, a.fileName + ".sql")
                } catch (b) { H(a.fileName + ".sql", "data:application/sql;charset=utf-8,", w) }
            } else if ("json" == a.type) {
                var S = []; G = []; p = c(v).find("thead").first().find(a.theadSelector); p.each(function () { var a = []; B(this, "th,td", l, p.length, function (b, c, d) { a.push(z(b, c, d)) }); S.push(a) }); var ia = []; t = u(c(v)); c(t).each(function () {
                    var a = {}, e = 0; B(this, "td,th", l, p.length + t.length, function (b, c, g) { S.length ? a[S[S.length - 1][e]] = z(b, c, g) : a[e] = z(b, c, g); e++ }); !1 === c.isEmptyObject(a) &&
                    ia.push(a); l++
                }); f = ""; f = "head" == a.jsonScope ? JSON.stringify(S) : "data" == a.jsonScope ? JSON.stringify(ia) : JSON.stringify({ header: S, data: ia }); !0 === a.consoleLog && console.log(f); if ("string" === a.outputMode) return f; if ("base64" === a.outputMode) return L(f); try { A = new Blob([f], { type: "application/json;charset=utf-8" }), saveAs(A, a.fileName + ".json") } catch (b) { H(a.fileName + ".json", "data:application/json;charset=utf-8;base64,", f) }
            } else if ("xml" === a.type) {
                l = 0; G = []; var J = '<?xml version="1.0" encoding="utf-8"?>'; J += "<tabledata><fields>";
                p = c(v).find("thead").first().find(a.theadSelector); p.each(function () { B(this, "th,td", l, p.length, function (a, c, d) { J += "<field>" + z(a, c, d) + "</field>" }); l++ }); J += "</fields><data>"; var ta = 1; t = u(c(v)); c(t).each(function () { var a = 1; m = ""; B(this, "td,th", l, p.length + t.length, function (b, c, d) { m += "<column-" + a + ">" + z(b, c, d) + "</column-" + a + ">"; a++ }); 0 < m.length && "<column-1></column-1>" != m && (J += '<row id="' + ta + '">' + m + "</row>", ta++); l++ }); J += "</data></tabledata>"; !0 === a.consoleLog && console.log(J); if ("string" === a.outputMode) return J;
                if ("base64" === a.outputMode) return L(J); try { A = new Blob([J], { type: "application/xml;charset=utf-8" }), saveAs(A, a.fileName + ".xml") } catch (b) { H(a.fileName + ".xml", "data:application/xml;charset=utf-8;base64,", J) }
            } else if ("excel" === a.type && "xmlss" === a.excelFileFormat) {
                var ja = [], F = []; c(v).filter(function () { return P(c(this)) }).each(function () {
                    function b(a, b, e) {
                        var g = []; c(a).each(function () {
                            var b = 0, h = 0; m = ""; B(this, "td,th", l, e + a.length, function (a, e, d) {
                                if (null !== a) {
                                    var k = ""; e = z(a, e, d); d = "String"; if (!1 !== jQuery.isNumeric(e)) d =
                                    "Number"; else { var f = Ba(e); !1 !== f && (e = f, d = "Number", k += ' ss:StyleID="pct1"') } "Number" !== d && (e = e.replace(/\n/g, "<br>")); f = parseInt(a.getAttribute("colspan")); a = parseInt(a.getAttribute("rowspan")); g.forEach(function (a) { if (l >= a.s.r && l <= a.e.r && h >= a.s.c && h <= a.e.c) for (var c = 0; c <= a.e.c - a.s.c; ++c) h++, b++ }); if (a || f) a = a || 1, f = f || 1, g.push({ s: { r: l, c: h }, e: { r: l + a - 1, c: h + f - 1 } }); 1 < f && (k += ' ss:MergeAcross="' + (f - 1) + '"', h += f - 1); 1 < a && (k += ' ss:MergeDown="' + (a - 1) + '" ss:StyleID="rsp1"'); 0 < b && (k += ' ss:Index="' + (h + 1) + '"', b = 0);
                                    m += "<Cell" + k + '><Data ss:Type="' + d + '">' + c("<div />").text(e).html() + "</Data></Cell>\r"; h++
                                }
                            }); 0 < m.length && (E += '<Row ss:AutoFitHeight="0">\r' + m + "</Row>\r"); l++
                        }); return a.length
                    } var e = c(this), d = ""; "string" === typeof a.worksheetName && a.worksheetName.length ? d = a.worksheetName + " " + (F.length + 1) : "undefined" !== typeof a.worksheetName[F.length] && (d = a.worksheetName[F.length]); d.length || (d = e.find("caption").text() || ""); d.length || (d = "Table " + (F.length + 1)); d = d.replace(/[\\\/[\]*:?'"]/g, "").substring(0, 31).trim();
                    F.push(c("<div />").text(d).html()); !1 === a.exportHiddenCells && (K = e.find("tr, th, td").filter(":hidden"), R = 0 < K.length); l = 0; Q = O(this); E = "<Table>\r"; d = 0; d += b(e.find("thead").first().find(a.theadSelector), "th,td", d); b(u(e), "td,th", d); E += "</Table>\r"; ja.push(E); !0 === a.consoleLog && console.log(E)
                }); f = {}; for (var y = {}, n, N, T = 0, Y = F.length; T < Y; T++) n = F[T], N = f[n], N = f[n] = null == N ? 1 : N + 1, 2 == N && (F[y[n]] = F[y[n]].substring(0, 29) + "-1"), 1 < f[n] ? F[T] = F[T].substring(0, 29) + "-" + f[n] : y[n] = T; f = '<?xml version="1.0" encoding="UTF-8"?>\r<?mso-application progid="Excel.Sheet"?>\r<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"\r xmlns:o="urn:schemas-microsoft-com:office:office"\r xmlns:x="urn:schemas-microsoft-com:office:excel"\r xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"\r xmlns:html="http://www.w3.org/TR/REC-html40">\r<DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">\r  <Created>' +
                (new Date).toISOString() + '</Created>\r</DocumentProperties>\r<OfficeDocumentSettings xmlns="urn:schemas-microsoft-com:office:office">\r  <AllowPNG/>\r</OfficeDocumentSettings>\r<ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">\r  <WindowHeight>9000</WindowHeight>\r  <WindowWidth>13860</WindowWidth>\r  <WindowTopX>0</WindowTopX>\r  <WindowTopY>0</WindowTopY>\r  <ProtectStructure>False</ProtectStructure>\r  <ProtectWindows>False</ProtectWindows>\r</ExcelWorkbook>\r<Styles>\r  <Style ss:ID="Default" ss:Name="Normal">\r    <Alignment ss:Vertical="Bottom"/>\r    <Borders/>\r    <Font/>\r    <Interior/>\r    <NumberFormat/>\r    <Protection/>\r  </Style>\r  <Style ss:ID="rsp1">\r    <Alignment ss:Vertical="Center"/>\r  </Style>\r  <Style ss:ID="pct1">\r    <NumberFormat ss:Format="Percent"/>\r  </Style>\r</Styles>\r';
                for (y = 0; y < ja.length; y++) f += '<Worksheet ss:Name="' + F[y] + '" ss:RightToLeft="' + (a.excelRTL ? "1" : "0") + '">\r' + ja[y], f = a.excelRTL ? f + '<WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">\r<DisplayRightToLeft/>\r</WorksheetOptions>\r' : f + '<WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel"/>\r', f += "</Worksheet>\r"; f += "</Workbook>\r"; !0 === a.consoleLog && console.log(f); if ("string" === a.outputMode) return f; if ("base64" === a.outputMode) return L(f); try {
                    A = new Blob([f], { type: "application/xml;charset=utf-8" }),
                    saveAs(A, a.fileName + ".xml")
                } catch (b) { H(a.fileName + ".xml", "data:application/xml;charset=utf-8;base64,", f) }
            } else if ("excel" == a.type || "xls" == a.type || "word" == a.type || "doc" == a.type) {
                f = "excel" == a.type || "xls" == a.type ? "excel" : "word"; y = "excel" == f ? "xls" : "doc"; n = 'xmlns:x="urn:schemas-microsoft-com:office:' + f + '"'; var E = "", V = ""; c(v).filter(function () { return P(c(this)) }).each(function () {
                    var b = c(this); "" === V && (V = a.worksheetName || b.find("caption").text() || "Table", V = V.replace(/[\\\/[\]*:?'"]/g, "").substring(0, 31).trim());
                    !1 === a.exportHiddenCells && (K = b.find("tr, th, td").filter(":hidden"), R = 0 < K.length); l = 0; G = []; Q = O(this); E += "<table><thead>"; p = b.find("thead").first().find(a.theadSelector); p.each(function () {
                        m = ""; B(this, "th,td", l, p.length, function (b, d, f) {
                            if (null !== b) {
                                var e = ""; m += "<th"; for (var h in a.excelstyles) if (a.excelstyles.hasOwnProperty(h)) { var k = c(b).css(a.excelstyles[h]); "" !== k && "0px none rgb(0, 0, 0)" != k && "rgba(0, 0, 0, 0)" != k && (e += "" === e ? 'style="' : ";", e += a.excelstyles[h] + ":" + k) } "" !== e && (m += " " + e + '"'); c(b).is("[colspan]") &&
                                (m += ' colspan="' + c(b).attr("colspan") + '"'); c(b).is("[rowspan]") && (m += ' rowspan="' + c(b).attr("rowspan") + '"'); m += ">" + z(b, d, f) + "</th>"
                            }
                        }); 0 < m.length && (E += "<tr>" + m + "</tr>"); l++
                    }); E += "</thead><tbody>"; t = u(b); c(t).each(function () {
                        var b = c(this); m = ""; B(this, "td,th", l, p.length + t.length, function (e, d, g) {
                            if (null !== e) {
                                var h = z(e, d, g), k = "", f = c(e).data("tableexport-msonumberformat"); "undefined" == typeof f && "function" === typeof a.onMsoNumberFormat && (f = a.onMsoNumberFormat(e, d, g)); "undefined" != typeof f && "" !== f && (k = "style=\"mso-number-format:'" +
                                f + "'"); for (var l in a.excelstyles) a.excelstyles.hasOwnProperty(l) && (f = c(e).css(a.excelstyles[l]), "" === f && (f = b.css(a.excelstyles[l])), "" !== f && "0px none rgb(0, 0, 0)" != f && "rgba(0, 0, 0, 0)" != f && (k += "" === k ? 'style="' : ";", k += a.excelstyles[l] + ":" + f)); m += "<td"; "" !== k && (m += " " + k + '"'); c(e).is("[colspan]") && (m += ' colspan="' + c(e).attr("colspan") + '"'); c(e).is("[rowspan]") && (m += ' rowspan="' + c(e).attr("rowspan") + '"'); "string" === typeof h && "" != h && (h = h.replace(/\n/g, "<br>")); m += ">" + h + "</td>"
                            }
                        }); 0 < m.length && (E += "<tr>" +
                        m + "</tr>"); l++
                    }); a.displayTableName && (E += "<tr><td></td></tr><tr><td></td></tr><tr><td>" + z(c("<p>" + a.tableName + "</p>")) + "</td></tr>"); E += "</tbody></table>"; !0 === a.consoleLog && console.log(E)
                }); n = '<html xmlns:o="urn:schemas-microsoft-com:office:office" ' + n + ' xmlns="http://www.w3.org/TR/REC-html40">' + ('<meta http-equiv="content-type" content="application/vnd.ms-' + f + '; charset=UTF-8">') + "<head>"; "excel" === f && (n += "\x3c!--[if gte mso 9]>", n += "<xml>", n += "<x:ExcelWorkbook>", n += "<x:ExcelWorksheets>", n += "<x:ExcelWorksheet>",
                n += "<x:Name>", n += V, n += "</x:Name>", n += "<x:WorksheetOptions>", n += "<x:DisplayGridlines/>", a.excelRTL && (n += "<x:DisplayRightToLeft/>"), n += "</x:WorksheetOptions>", n += "</x:ExcelWorksheet>", n += "</x:ExcelWorksheets>", n += "</x:ExcelWorkbook>", n += "</xml>", n += "<![endif]--\x3e"); n += "<style>br {mso-data-placement:same-cell;}</style>"; n += "</head>"; n += "<body>"; n += E; n += "</body>"; n += "</html>"; !0 === a.consoleLog && console.log(n); if ("string" === a.outputMode) return n; if ("base64" === a.outputMode) return L(n); try {
                    A = new Blob([n],
                    { type: "application/vnd.ms-" + a.type }), saveAs(A, a.fileName + "." + y)
                } catch (b) { H(a.fileName + "." + y, "data:application/vnd.ms-" + f + ";base64,", n) }
            } else if ("xlsx" == a.type) {
                var ua = [], ka = []; l = 0; t = c(v).find("thead").first().find(a.theadSelector); t.push.apply(t, u(c(v))); c(t).each(function () {
                    var b = []; B(this, "th,td", l, t.length, function (c, d, f) {
                        if ("undefined" !== typeof c && null !== c) {
                            f = z(c, d, f); d = parseInt(c.getAttribute("colspan")); c = parseInt(c.getAttribute("rowspan")); ka.forEach(function (a) {
                                if (l >= a.s.r && l <= a.e.r && b.length >=
                                a.s.c && b.length <= a.e.c) for (var c = 0; c <= a.e.c - a.s.c; ++c) b.push(null)
                            }); if (c || d) d = d || 1, ka.push({ s: { r: l, c: b.length }, e: { r: l + (c || 1) - 1, c: b.length + d - 1 } }); "function" !== typeof a.onCellData && "" !== f && f == +f && (f = +f); b.push("" !== f ? f : null); if (d) for (c = 0; c < d - 1; ++c) b.push(null)
                        }
                    }); ua.push(b); l++
                }); f = new fa; y = Da(ua); y["!merges"] = ka; f.SheetNames.push(a.worksheetName); f.Sheets[a.worksheetName] = y; f = XLSX.write(f, { bookType: a.type, bookSST: !1, type: "binary" }); try {
                    A = new Blob([sa(f)], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8" }),
                    saveAs(A, a.fileName + "." + a.type)
                } catch (b) { H(a.fileName + "." + a.type, "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8,", sa(f)) }
            } else if ("png" == a.type) html2canvas(c(v)[0]).then(function (b) {
                b = b.toDataURL(); for (var c = atob(b.substring(22)), d = new ArrayBuffer(c.length), f = new Uint8Array(d), g = 0; g < c.length; g++) f[g] = c.charCodeAt(g); !0 === a.consoleLog && console.log(c); if ("string" === a.outputMode) return c; if ("base64" === a.outputMode) return L(b); if ("window" === a.outputMode) window.open(b);
                else try { A = new Blob([d], { type: "image/png" }), saveAs(A, a.fileName + ".png") } catch (h) { H(a.fileName + ".png", "data:image/png,", A) }
            }); else if ("pdf" == a.type) if (!0 === a.pdfmake.enabled) {
                f = []; var va = []; l = 0; G = []; y = function (a, d, f) {
                    var b = 0; c(a).each(function () {
                        var a = []; B(this, d, l, f, function (b, c, d) { if ("undefined" !== typeof b && null !== b) { var e = parseInt(b.getAttribute("colspan")), g = parseInt(b.getAttribute("rowspan")); b = z(b, c, d) || " "; 1 < e || 1 < g ? a.push({ colSpan: e || 1, rowSpan: g || 1, text: b }) : a.push(b) } else a.push(" ") }); a.length &&
                        va.push(a); b < a.length && (b = a.length); l++
                    }); return b
                }; p = c(this).find("thead").first().find(a.theadSelector); n = y(p, "th,td", p.length); for (N = f.length; N < n; N++) f.push("*"); t = u(c(this)); y(t, "th,td", p.length + t.length); f = { content: [{ table: { headerRows: p.length, widths: f, body: va } }] }; c.extend(!0, f, a.pdfmake.docDefinition); pdfMake.fonts = { Roboto: { normal: "Roboto-Regular.ttf", bold: "Roboto-Medium.ttf", italics: "Roboto-Italic.ttf", bolditalics: "Roboto-MediumItalic.ttf" } }; c.extend(!0, pdfMake.fonts, a.pdfmake.fonts); pdfMake.createPdf(f).getBuffer(function (b) {
                    try {
                        var c =
                        new Blob([b], { type: "application/pdf" }); saveAs(c, a.fileName + ".pdf")
                    } catch (k) { H(a.fileName + ".pdf", "data:application/pdf;base64,", b) }
                })
            } else if (!1 === a.jspdf.autotable) { f = { dim: { w: aa(c(v).first().get(0), "width", "mm"), h: aa(c(v).first().get(0), "height", "mm") }, pagesplit: !1 }; var wa = new jsPDF(a.jspdf.orientation, a.jspdf.unit, a.jspdf.format); wa.addHTML(c(v).first(), a.jspdf.margins.left, a.jspdf.margins.top, f, function () { la(wa, !1) }) } else {
                var d = a.jspdf.autotable.tableExport; if ("string" === typeof a.jspdf.format &&
                "bestfit" === a.jspdf.format.toLowerCase()) { var W = { a0: [2383.94, 3370.39], a1: [1683.78, 2383.94], a2: [1190.55, 1683.78], a3: [841.89, 1190.55], a4: [595.28, 841.89] }, da = "", X = "", xa = 0; c(v).each(function () { if (P(c(this))) { var a = aa(c(this).get(0), "width", "pt"); if (a > xa) { a > W.a0[0] && (da = "a0", X = "l"); for (var d in W) W.hasOwnProperty(d) && W[d][1] > a && (da = d, X = "l", W[d][0] > a && (X = "p")); xa = a } } }); a.jspdf.format = "" === da ? "a4" : da; a.jspdf.orientation = "" === X ? "w" : X } if (null == d.doc && (d.doc = new jsPDF(a.jspdf.orientation, a.jspdf.unit, a.jspdf.format),
                "function" === typeof a.jspdf.onDocCreated)) a.jspdf.onDocCreated(d.doc); !0 === d.outputImages && (d.images = {}); "undefined" != typeof d.images && (c(v).filter(function () { return P(c(this)) }).each(function () {
                    var b = 0; G = []; !1 === a.exportHiddenCells && (K = c(this).find("tr, th, td").filter(":hidden"), R = 0 < K.length); p = c(this).find("thead").find(a.theadSelector); t = u(c(this)); c(t).each(function () {
                        B(this, "td,th", p.length + b, p.length + t.length, function (a) {
                            if ("undefined" !== typeof a && null !== a) {
                                var b = c(a).children(); "undefined" !=
                                typeof b && 0 < b.length && na(a, b, d)
                            }
                        }); b++
                    })
                }), p = [], t = []); Aa(d, function () {
                    c(v).filter(function () { return P(c(this)) }).each(function () {
                        var b; l = 0; G = []; !1 === a.exportHiddenCells && (K = c(this).find("tr, th, td").filter(":hidden"), R = 0 < K.length); Q = O(this); d.columns = []; d.rows = []; d.rowoptions = {}; if ("function" === typeof d.onTable && !1 === d.onTable(c(this), a)) return !0; a.jspdf.autotable.tableExport = null; var e = c.extend(!0, {}, a.jspdf.autotable); a.jspdf.autotable.tableExport = d; e.margin = {}; c.extend(!0, e.margin, a.jspdf.margins);
                        e.tableExport = d; "function" !== typeof e.beforePageContent && (e.beforePageContent = function (a) { 1 == a.pageCount && a.table.rows.concat(a.table.headerRow).forEach(function (b) { 0 < b.height && (b.height += (2 - 1.15) / 2 * b.styles.fontSize, a.table.height += (2 - 1.15) / 2 * b.styles.fontSize) }) }); "function" !== typeof e.createdHeaderCell && (e.createdHeaderCell = function (a, b) {
                            a.styles = c.extend({}, b.row.styles); if ("undefined" != typeof d.columns[b.column.dataKey]) {
                                var g = d.columns[b.column.dataKey]; if ("undefined" != typeof g.rect) {
                                    a.contentWidth =
                                    g.rect.width; if ("undefined" == typeof d.heightRatio || 0 === d.heightRatio) { var f = b.row.raw[b.column.dataKey].rowspan ? b.row.raw[b.column.dataKey].rect.height / b.row.raw[b.column.dataKey].rowspan : b.row.raw[b.column.dataKey].rect.height; d.heightRatio = a.styles.rowHeight / f } f = b.row.raw[b.column.dataKey].rect.height * d.heightRatio; f > a.styles.rowHeight && (a.styles.rowHeight = f)
                                } "undefined" != typeof g.style && !0 !== g.style.hidden && (a.styles.halign = g.style.align, "inherit" === e.styles.fillColor && (a.styles.fillColor = g.style.bcolor),
                                "inherit" === e.styles.textColor && (a.styles.textColor = g.style.color), "inherit" === e.styles.fontStyle && (a.styles.fontStyle = g.style.fstyle))
                            }
                        }); "function" !== typeof e.createdCell && (e.createdCell = function (a, b) {
                            b = d.rowoptions[b.row.index + ":" + b.column.dataKey]; "undefined" != typeof b && "undefined" != typeof b.style && !0 !== b.style.hidden && (a.styles.halign = b.style.align, "inherit" === e.styles.fillColor && (a.styles.fillColor = b.style.bcolor), "inherit" === e.styles.textColor && (a.styles.textColor = b.style.color), "inherit" ===
                            e.styles.fontStyle && (a.styles.fontStyle = b.style.fstyle))
                        }); "function" !== typeof e.drawHeaderCell && (e.drawHeaderCell = function (a, b) { var c = d.columns[b.column.dataKey]; return (!0 !== c.style.hasOwnProperty("hidden") || !0 !== c.style.hidden) && 0 <= c.rowIndex ? ma(a, b, c) : !1 }); "function" !== typeof e.drawCell && (e.drawCell = function (a, b) {
                            var c = d.rowoptions[b.row.index + ":" + b.column.dataKey]; if (ma(a, b, c)) if (d.doc.rect(a.x, a.y, a.width, a.height, a.styles.fillStyle), "undefined" != typeof c && "undefined" != typeof c.kids && 0 < c.kids.length) {
                                b =
                                a.height / c.rect.height; if (b > d.dh || "undefined" == typeof d.dh) d.dh = b; d.dw = a.width / c.rect.width; b = a.textPos.y; pa(a, c.kids, d); a.textPos.y = b; qa(a, c.kids, d)
                            } else qa(a, {}, d); return !1
                        }); d.headerrows = []; p = c(this).find("thead").find(a.theadSelector); p.each(function () { b = 0; d.headerrows[l] = []; B(this, "th,td", l, p.length, function (a, c, e) { var f = ra(a); f.title = z(a, c, e); f.key = b++; f.rowIndex = l; d.headerrows[l].push(f) }); l++ }); if (0 < l) for (var f = l - 1; 0 <= f;) c.each(d.headerrows[f], function () {
                            var a = this; 0 < f && null === this.rect &&
                            (a = d.headerrows[f - 1][this.key]); null !== a && 0 <= a.rowIndex && (!0 !== a.style.hasOwnProperty("hidden") || !0 !== a.style.hidden) && d.columns.push(a)
                        }), f = 0 < d.columns.length ? -1 : f - 1; var m = 0; t = []; t = u(c(this)); c(t).each(function () {
                            var a = []; b = 0; B(this, "td,th", l, p.length + t.length, function (e, f, g) {
                                if ("undefined" === typeof d.columns[b]) { var h = { title: "", key: b, style: { hidden: !0 } }; d.columns.push(h) } "undefined" !== typeof e && null !== e ? (h = ra(e), h.kids = c(e).children()) : (h = c.extend(!0, {}, d.rowoptions[m + ":" + (b - 1)]), h.colspan = -1); d.rowoptions[m +
                                ":" + b++] = h; a.push(z(e, f, g))
                            }); a.length && (d.rows.push(a), m++); l++
                        }); if ("function" === typeof d.onBeforeAutotable) d.onBeforeAutotable(c(this), d.columns, d.rows, e); d.doc.autoTable(d.columns, d.rows, e); if ("function" === typeof d.onAfterAutotable) d.onAfterAutotable(c(this), e); a.jspdf.autotable.startY = d.doc.autoTableEndPosY() + e.margin.top
                    }); la(d.doc, "undefined" != typeof d.images && !1 === jQuery.isEmptyObject(d.images)); "undefined" != typeof d.headerrows && (d.headerrows.length = 0); "undefined" != typeof d.columns && (d.columns.length =
                    0); "undefined" != typeof d.rows && (d.rows.length = 0); delete d.doc; d.doc = null
                })
            } return this
        }
    })
})(jQuery);

   

function export_excel(boostrap_table_class_name, temp_container) {
    var all_data_js = $('.' + boostrap_table_class_name).bootstrapTable('getData', false);
    if (all_data_js.length > 0) {
        var col_name_array = Object.keys(all_data_js[0]);
        var col_name_array1 = [];
        var result_arr = [];
        for (var i = 0; i < col_name_array.length; i++) {

            if (isFinite(String(col_name_array[i]))) { }
            else
                if (col_name_array[i] != "select") {
                    col_name_array1.push(col_name_array[i]);
                }
        }

        var str1 = '{';
        for (var i = 0; i < col_name_array1.length; i++) {
            if (i == col_name_array1.length - 1) { str1 += '\"' + col_name_array1[i] + '\":\"\"}' }
            else str1 += '\"' + col_name_array1[i] + '\":\"\",';
        }


        for (var i = 0; i < all_data_js.length; i++) {
            var object_mau = {};
            for (var j = 0; j < col_name_array1.length; j++) {
                if (all_data_js[i][col_name_array1[j]] == undefined || all_data_js[i][col_name_array1[j]] == null)
                    all_data_js[i][col_name_array1[j]] = "";
                object_mau[col_name_array1[j]] = all_data_js[i][col_name_array1[j]];
            }
            result_arr.push(object_mau);
        }
        str1 = '';
        for (var i = 0; i < col_name_array1.length; i++) {
            str1 += '<th>' + col_name_array1[i] + '</th>';
        }
        str1 = '<thead><tr>' + str1 + '</tr></thead><tbody>';

        for (var i = 0; i < result_arr.length; i++) {
            var str2 = '';
            for (var j = 0; j < col_name_array1.length; j++) {
                str2 += '<td>' + result_arr[i][col_name_array1[j]] + '</td>';
            }
            str2 = '<tr>' + str2 + '</tr>';
            str1 += str2;
        }
        str1 = '<table class="table table-hover">' + str1 + '</tbody></table>';
        $("." + temp_container).html('');
        $("." + temp_container).html(str1);
        //$("." + temp_container).find('table').tableExport({
        //    type: 'excel',
        //    excelFileFormat: 'xmlss',
        //    worksheetName: ['Table 1', 'Table 2', 'Table 3']
        //});
        //   $("#aaa").children("table").tableExport({ type: 'excel' });
        if ($("." + temp_container).html().trim() != "")
            $("." + temp_container).find('table').tableExport({ type: 'excel' });
    }
}

