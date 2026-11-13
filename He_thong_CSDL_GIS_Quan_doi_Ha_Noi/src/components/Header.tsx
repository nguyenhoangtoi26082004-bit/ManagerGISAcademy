const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"];const _jsxDEV = __vite__cjsImport2_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { Search, Database, Plus, Download } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/Header.tsx";
import __vite__cjsImport2_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const Header = ({ onQuickSearch, onOpenAddModal, onOpenPostgisModal, onDownloadZip, unitsCount, postgisConnected }) => {
	const [searchValue, setSearchValue] = useState("");
	const handleSubmit = (e) => {
		e.preventDefault();
		onQuickSearch(searchValue);
	};
	return /* @__PURE__ */ _jsxDEV("header", {
		className: "bg-[#0b1324] border-b border-[#1e293b] text-white px-4 py-2.5 flex flex-wrap items-center justify-between gap-3 shadow-lg select-none z-30",
		children: [/* @__PURE__ */ _jsxDEV("div", {
			className: "flex items-center space-x-3.5",
			children: [/* @__PURE__ */ _jsxDEV("div", {
				className: "relative flex-shrink-0 w-11 h-11 rounded-full bg-gradient-to-br from-red-700 via-amber-600 to-emerald-800 p-0.5 shadow-md flex items-center justify-center",
				children: /* @__PURE__ */ _jsxDEV("div", {
					className: "w-full h-full rounded-full bg-[#09101d] flex flex-col items-center justify-center border border-amber-400/40 relative overflow-hidden",
					children: /* @__PURE__ */ _jsxDEV("svg", {
						viewBox: "0 0 100 100",
						className: "w-8 h-8",
						children: [
							/* @__PURE__ */ _jsxDEV("circle", {
								cx: "50",
								cy: "50",
								r: "44",
								stroke: "#fbbf24",
								strokeWidth: "2.5",
								fill: "none",
								strokeDasharray: "6 3"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 37,
								columnNumber: 15
							}, this),
							/* @__PURE__ */ _jsxDEV("circle", {
								cx: "50",
								cy: "50",
								r: "34",
								stroke: "#e11d48",
								strokeWidth: "1.5",
								fill: "#1e293b"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 39,
								columnNumber: 15
							}, this),
							/* @__PURE__ */ _jsxDEV("polygon", {
								points: "50,22 58,38 76,38 62,49 67,66 50,55 33,66 38,49 24,38 42,38",
								fill: "#fbbf24"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 41,
								columnNumber: 15
							}, this),
							/* @__PURE__ */ _jsxDEV("ellipse", {
								cx: "50",
								cy: "50",
								rx: "30",
								ry: "10",
								stroke: "#38bdf8",
								strokeWidth: "1.2",
								fill: "none",
								transform: "rotate(30 50 50)",
								opacity: "0.8"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 46,
								columnNumber: 15
							}, this),
							/* @__PURE__ */ _jsxDEV("ellipse", {
								cx: "50",
								cy: "50",
								rx: "30",
								ry: "10",
								stroke: "#38bdf8",
								strokeWidth: "1.2",
								fill: "none",
								transform: "rotate(-30 50 50)",
								opacity: "0.8"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 47,
								columnNumber: 15
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 35,
						columnNumber: 13
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 34,
					columnNumber: 11
				}, this)
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 33,
				columnNumber: 9
			}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("div", {
				className: "flex items-center gap-2",
				children: [/* @__PURE__ */ _jsxDEV("h1", {
					className: "text-base sm:text-lg font-bold uppercase tracking-wider text-white flex items-center gap-2",
					children: /* @__PURE__ */ _jsxDEV("span", { children: "HỆ THỐNG CSDL GIS QUẢN LÝ ĐƠN VỊ HÀ NỘI" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 55,
						columnNumber: 15
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 54,
					columnNumber: 13
				}, this), /* @__PURE__ */ _jsxDEV("span", {
					className: "hidden md:inline-flex items-center px-2 py-0.5 text-[10px] font-semibold bg-emerald-950 text-emerald-400 border border-emerald-700/60 rounded",
					children: "EPSG:32648"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 57,
					columnNumber: 13
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 53,
				columnNumber: 11
			}, this), /* @__PURE__ */ _jsxDEV("p", {
				className: "text-xs text-slate-400 hidden sm:block",
				children: "Tra cứu thông tin, quản lý dữ liệu không gian và phân tích GIS trên địa bàn Hà Nội"
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 61,
				columnNumber: 11
			}, this)] }, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 52,
				columnNumber: 9
			}, this)]
		}, void 0, true, {
			fileName: _jsxFileName,
			lineNumber: 31,
			columnNumber: 7
		}, this), /* @__PURE__ */ _jsxDEV("div", {
			className: "flex items-center gap-3 ml-auto",
			children: [
				/* @__PURE__ */ _jsxDEV("form", {
					onSubmit: handleSubmit,
					className: "flex items-center",
					children: /* @__PURE__ */ _jsxDEV("div", {
						className: "flex flex-col",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-[10px] uppercase font-bold text-slate-400 tracking-wider mb-0.5 hidden md:block",
							children: "TRA CỨU NHANH"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 72,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center bg-[#090f1d] border border-slate-700/80 rounded focus-within:border-emerald-500 overflow-hidden shadow-inner",
							children: [/* @__PURE__ */ _jsxDEV("input", {
								type: "text",
								placeholder: "Tên đơn vị, quận huyện, mã...",
								value: searchValue,
								onChange: (e) => setSearchValue(e.target.value),
								className: "bg-transparent px-3 py-1.5 text-xs text-white placeholder-slate-500 focus:outline-none w-44 md:w-64"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 76,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("button", {
								type: "submit",
								className: "bg-emerald-600 hover:bg-emerald-500 text-white px-3 py-1.5 text-xs font-medium flex items-center gap-1 transition-colors",
								children: [/* @__PURE__ */ _jsxDEV(Search, { className: "w-3.5 h-3.5" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 87,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tìm" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 88,
									columnNumber: 17
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 83,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 75,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 71,
						columnNumber: 11
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 70,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("button", {
					onClick: onOpenPostgisModal,
					title: "Xem trạng thái và cấu trúc Cơ sở dữ liệu PostGIS",
					className: "flex items-center gap-1.5 px-3 py-2 text-xs rounded border border-slate-700 bg-slate-800/80 hover:bg-slate-700/80 text-slate-300 transition",
					children: [
						/* @__PURE__ */ _jsxDEV("span", { className: `w-2 h-2 rounded-full ${postgisConnected ? "bg-emerald-400 animate-pulse" : "bg-amber-400"}` }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 100,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV(Database, { className: "w-3.5 h-3.5 text-slate-400" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 105,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("span", {
							className: "font-mono text-xs",
							children: postgisConnected ? "PostGIS | Sẵn sàng" : "PostGIS | Đang kết nối"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 106,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 95,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("button", {
					onClick: onDownloadZip,
					title: "Tải về toàn bộ mã nguồn dự án (.zip) để chạy trên máy tính cá nhân",
					className: "bg-amber-600 hover:bg-amber-500 active:scale-95 text-white text-xs font-semibold px-3 py-2 rounded flex items-center gap-1.5 shadow transition-all",
					children: [/* @__PURE__ */ _jsxDEV(Download, { className: "w-4 h-4 stroke-[2.2]" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 117,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tải dự án (.zip)" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 118,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 112,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("button", {
					onClick: onOpenAddModal,
					className: "bg-emerald-600 hover:bg-emerald-500 active:scale-95 text-white text-xs font-semibold px-3.5 py-2 rounded flex items-center gap-1.5 shadow transition-all",
					children: [/* @__PURE__ */ _jsxDEV(Plus, { className: "w-4 h-4 stroke-[2.5]" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 126,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Thêm đơn vị" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 127,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 122,
					columnNumber: 9
				}, this)
			]
		}, void 0, true, {
			fileName: _jsxFileName,
			lineNumber: 68,
			columnNumber: 7
		}, this)]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 29,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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LFVBQUQsRUFBVSxXQUFVLHVCQUF3Qjs7OztlQUM1Qyx3QkFBQyxRQUFELFlBQU0sbUJBQXNCOzs7O2FBQ3RCOzs7Ozs7SUFHUix3QkFBQyxVQUFEO0tBQ0UsU0FBUztLQUNULFdBQVU7ZUFGWixDQUlFLHdCQUFDLE1BQUQsRUFBTSxXQUFVLHVCQUF3Qjs7OztlQUN4Qyx3QkFBQyxRQUFELFlBQU0sY0FBaUI7Ozs7YUFDakI7Ozs7OztHQUNMOzs7OztVQUNDOzs7Ozs7QUFFWiIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJIZWFkZXIudHN4Il0sInZlcnNpb24iOjMsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBSZWFjdCwgeyB1c2VTdGF0ZSB9IGZyb20gJ3JlYWN0JztcbmltcG9ydCB7IFNlYXJjaCwgRGF0YWJhc2UsIFBsdXMsIFNoaWVsZCwgTWFwUGluLCBEb3dubG9hZCB9IGZyb20gJ2x1Y2lkZS1yZWFjdCc7XG5cbmludGVyZmFjZSBIZWFkZXJQcm9wcyB7XG4gIG9uUXVpY2tTZWFyY2g6IChxdWVyeTogc3RyaW5nKSA9PiB2b2lkO1xuICBvbk9wZW5BZGRNb2RhbDogKCkgPT4gdm9pZDtcbiAgb25PcGVuUG9zdGdpc01vZGFsOiAoKSA9PiB2b2lkO1xuICBvbkRvd25sb2FkWmlwOiAoKSA9PiB2b2lkO1xuICB1bml0c0NvdW50OiBudW1iZXI7XG4gIHBvc3RnaXNDb25uZWN0ZWQ6IGJvb2xlYW47XG59XG5cbmV4cG9ydCBjb25zdCBIZWFkZXI6IFJlYWN0LkZDPEhlYWRlclByb3BzPiA9ICh7XG4gIG9uUXVpY2tTZWFyY2gsXG4gIG9uT3BlbkFkZE1vZGFsLFxuICBvbk9wZW5Qb3N0Z2lzTW9kYWwsXG4gIG9uRG93bmxvYWRaaXAsXG4gIHVuaXRzQ291bnQsXG4gIHBvc3RnaXNDb25uZWN0ZWQsXG59KSA9PiB7XG4gIGNvbnN0IFtzZWFyY2hWYWx1ZSwgc2V0U2VhcmNoVmFsdWVdID0gdXNlU3RhdGUoJycpO1xuXG4gIGNvbnN0IGhhbmRsZVN1Ym1pdCA9IChlOiBSZWFjdC5Gb3JtRXZlbnQpID0+IHtcbiAgICBlLnByZXZlbnREZWZhdWx0KCk7XG4gICAgb25RdWlja1NlYXJjaChzZWFyY2hWYWx1ZSk7XG4gIH07XG5cbiAgcmV0dXJuIChcbiAgICA8aGVhZGVyIGNsYXNzTmFtZT1cImJnLVsjMGIxMzI0XSBib3JkZXItYiBib3JkZXItWyMxZTI5M2JdIHRleHQtd2hpdGUgcHgtNCBweS0yLjUgZmxleCBmbGV4LXdyYXAgaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBnYXAtMyBzaGFkb3ctbGcgc2VsZWN0LW5vbmUgei0zMFwiPlxuICAgICAgey8qIEJyYW5kICYgRW1ibGVtICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBzcGFjZS14LTMuNVwiPlxuICAgICAgICB7LyogTVRBIEVtYmxlbSAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJyZWxhdGl2ZSBmbGV4LXNocmluay0wIHctMTEgaC0xMSByb3VuZGVkLWZ1bGwgYmctZ3JhZGllbnQtdG8tYnIgZnJvbS1yZWQtNzAwIHZpYS1hbWJlci02MDAgdG8tZW1lcmFsZC04MDAgcC0wLjUgc2hhZG93LW1kIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyXCI+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ3LWZ1bGwgaC1mdWxsIHJvdW5kZWQtZnVsbCBiZy1bIzA5MTAxZF0gZmxleCBmbGV4LWNvbCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgYm9yZGVyIGJvcmRlci1hbWJlci00MDAvNDAgcmVsYXRpdmUgb3ZlcmZsb3ctaGlkZGVuXCI+XG4gICAgICAgICAgICA8c3ZnIHZpZXdCb3g9XCIwIDAgMTAwIDEwMFwiIGNsYXNzTmFtZT1cInctOCBoLThcIj5cbiAgICAgICAgICAgICAgey8qIENvZyAvIEdlYXIgYm9yZGVyICovfVxuICAgICAgICAgICAgICA8Y2lyY2xlIGN4PVwiNTBcIiBjeT1cIjUwXCIgcj1cIjQ0XCIgc3Ryb2tlPVwiI2ZiYmYyNFwiIHN0cm9rZVdpZHRoPVwiMi41XCIgZmlsbD1cIm5vbmVcIiBzdHJva2VEYXNoYXJyYXk9XCI2IDNcIiAvPlxuICAgICAgICAgICAgICB7LyogSW5uZXIgcmluZyAqL31cbiAgICAgICAgICAgICAgPGNpcmNsZSBjeD1cIjUwXCIgY3k9XCI1MFwiIHI9XCIzNFwiIHN0cm9rZT1cIiNlMTFkNDhcIiBzdHJva2VXaWR0aD1cIjEuNVwiIGZpbGw9XCIjMWUyOTNiXCIgLz5cbiAgICAgICAgICAgICAgey8qIEdvbGRlbiBTdGFyICovfVxuICAgICAgICAgICAgICA8cG9seWdvblxuICAgICAgICAgICAgICAgIHBvaW50cz1cIjUwLDIyIDU4LDM4IDc2LDM4IDYyLDQ5IDY3LDY2IDUwLDU1IDMzLDY2IDM4LDQ5IDI0LDM4IDQyLDM4XCJcbiAgICAgICAgICAgICAgICBmaWxsPVwiI2ZiYmYyNFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICAgIHsvKiBFbGVjdHJvbmljIG9yYml0cyAvIHRlY2ggbGluZXMgKi99XG4gICAgICAgICAgICAgIDxlbGxpcHNlIGN4PVwiNTBcIiBjeT1cIjUwXCIgcng9XCIzMFwiIHJ5PVwiMTBcIiBzdHJva2U9XCIjMzhiZGY4XCIgc3Ryb2tlV2lkdGg9XCIxLjJcIiBmaWxsPVwibm9uZVwiIHRyYW5zZm9ybT1cInJvdGF0ZSgzMCA1MCA1MClcIiBvcGFjaXR5PVwiMC44XCIgLz5cbiAgICAgICAgICAgICAgPGVsbGlwc2UgY3g9XCI1MFwiIGN5PVwiNTBcIiByeD1cIjMwXCIgcnk9XCIxMFwiIHN0cm9rZT1cIiMzOGJkZjhcIiBzdHJva2VXaWR0aD1cIjEuMlwiIGZpbGw9XCJub25lXCIgdHJhbnNmb3JtPVwicm90YXRlKC0zMCA1MCA1MClcIiBvcGFjaXR5PVwiMC44XCIgLz5cbiAgICAgICAgICAgIDwvc3ZnPlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8ZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgIDxoMSBjbGFzc05hbWU9XCJ0ZXh0LWJhc2Ugc206dGV4dC1sZyBmb250LWJvbGQgdXBwZXJjYXNlIHRyYWNraW5nLXdpZGVyIHRleHQtd2hpdGUgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgICAgPHNwYW4+SOG7hiBUSOG7kE5HIENTREwgR0lTIFFV4bqiTiBMw50gxJDGoE4gVuG7iiBIw4AgTuG7mEk8L3NwYW4+XG4gICAgICAgICAgICA8L2gxPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwiaGlkZGVuIG1kOmlubGluZS1mbGV4IGl0ZW1zLWNlbnRlciBweC0yIHB5LTAuNSB0ZXh0LVsxMHB4XSBmb250LXNlbWlib2xkIGJnLWVtZXJhbGQtOTUwIHRleHQtZW1lcmFsZC00MDAgYm9yZGVyIGJvcmRlci1lbWVyYWxkLTcwMC82MCByb3VuZGVkXCI+XG4gICAgICAgICAgICAgIEVQU0c6MzI2NDhcbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8cCBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGhpZGRlbiBzbTpibG9ja1wiPlxuICAgICAgICAgICAgVHJhIGPhu6l1IHRow7RuZyB0aW4sIHF14bqjbiBsw70gZOG7ryBsaeG7h3Uga2jDtG5nIGdpYW4gdsOgIHBow6JuIHTDrWNoIEdJUyB0csOqbiDEkeG7i2EgYsOgbiBIw6AgTuG7mWlcbiAgICAgICAgICA8L3A+XG4gICAgICAgIDwvZGl2PlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBDZW50ZXIgU2VhcmNoICYgQWN0aW9ucyAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTMgbWwtYXV0b1wiPlxuICAgICAgICB7LyogUXVpY2sgU2VhcmNoICovfVxuICAgICAgICA8Zm9ybSBvblN1Ym1pdD17aGFuZGxlU3VibWl0fSBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlclwiPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBmbGV4LWNvbFwiPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdXBwZXJjYXNlIGZvbnQtYm9sZCB0ZXh0LXNsYXRlLTQwMCB0cmFja2luZy13aWRlciBtYi0wLjUgaGlkZGVuIG1kOmJsb2NrXCI+XG4gICAgICAgICAgICAgIFRSQSBD4buoVSBOSEFOSFxuICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAvODAgcm91bmRlZCBmb2N1cy13aXRoaW46Ym9yZGVyLWVtZXJhbGQtNTAwIG92ZXJmbG93LWhpZGRlbiBzaGFkb3ctaW5uZXJcIj5cbiAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgdHlwZT1cInRleHRcIlxuICAgICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiVMOqbiDEkcahbiB24buLLCBxdeG6rW4gaHV54buHbiwgbcOjLi4uXCJcbiAgICAgICAgICAgICAgICB2YWx1ZT17c2VhcmNoVmFsdWV9XG4gICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRTZWFyY2hWYWx1ZShlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiYmctdHJhbnNwYXJlbnQgcHgtMyBweS0xLjUgdGV4dC14cyB0ZXh0LXdoaXRlIHBsYWNlaG9sZGVyLXNsYXRlLTUwMCBmb2N1czpvdXRsaW5lLW5vbmUgdy00NCBtZDp3LTY0XCJcbiAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgICAgIHR5cGU9XCJzdWJtaXRcIlxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLWVtZXJhbGQtNjAwIGhvdmVyOmJnLWVtZXJhbGQtNTAwIHRleHQtd2hpdGUgcHgtMyBweS0xLjUgdGV4dC14cyBmb250LW1lZGl1bSBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMSB0cmFuc2l0aW9uLWNvbG9yc1wiXG4gICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICA8U2VhcmNoIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgICAgICA8c3Bhbj5Uw6xtPC9zcGFuPlxuICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Zvcm0+XG5cbiAgICAgICAgey8qIFBvc3RHSVMgU3RhdHVzIGluZGljYXRvciAmIHRyaWdnZXIgKi99XG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXtvbk9wZW5Qb3N0Z2lzTW9kYWx9XG4gICAgICAgICAgdGl0bGU9XCJYZW0gdHLhuqFuZyB0aMOhaSB2w6AgY+G6pXUgdHLDumMgQ8ahIHPhu58gZOG7ryBsaeG7h3UgUG9zdEdJU1wiXG4gICAgICAgICAgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEuNSBweC0zIHB5LTIgdGV4dC14cyByb3VuZGVkIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIGJnLXNsYXRlLTgwMC84MCBob3ZlcjpiZy1zbGF0ZS03MDAvODAgdGV4dC1zbGF0ZS0zMDAgdHJhbnNpdGlvblwiXG4gICAgICAgID5cbiAgICAgICAgICA8c3BhblxuICAgICAgICAgICAgY2xhc3NOYW1lPXtgdy0yIGgtMiByb3VuZGVkLWZ1bGwgJHtcbiAgICAgICAgICAgICAgcG9zdGdpc0Nvbm5lY3RlZCA/ICdiZy1lbWVyYWxkLTQwMCBhbmltYXRlLXB1bHNlJyA6ICdiZy1hbWJlci00MDAnXG4gICAgICAgICAgICB9YH1cbiAgICAgICAgICAvPlxuICAgICAgICAgIDxEYXRhYmFzZSBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LXNsYXRlLTQwMFwiIC8+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwiZm9udC1tb25vIHRleHQteHNcIj5cbiAgICAgICAgICAgIHtwb3N0Z2lzQ29ubmVjdGVkID8gJ1Bvc3RHSVMgfCBT4bq1biBzw6BuZycgOiAnUG9zdEdJUyB8IMSQYW5nIGvhur90IG7hu5FpJ31cbiAgICAgICAgICA8L3NwYW4+XG4gICAgICAgIDwvYnV0dG9uPlxuXG4gICAgICAgIHsvKiBEb3dubG9hZCBGdWxsIFByb2plY3QgWklQIEJ1dHRvbiAqL31cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9e29uRG93bmxvYWRaaXB9XG4gICAgICAgICAgdGl0bGU9XCJU4bqjaSB24buBIHRvw6BuIGLhu5kgbcOjIG5ndeG7k24gZOG7sSDDoW4gKC56aXApIMSR4buDIGNo4bqheSB0csOqbiBtw6F5IHTDrW5oIGPDoSBuaMOiblwiXG4gICAgICAgICAgY2xhc3NOYW1lPVwiYmctYW1iZXItNjAwIGhvdmVyOmJnLWFtYmVyLTUwMCBhY3RpdmU6c2NhbGUtOTUgdGV4dC13aGl0ZSB0ZXh0LXhzIGZvbnQtc2VtaWJvbGQgcHgtMyBweS0yIHJvdW5kZWQgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEuNSBzaGFkb3cgdHJhbnNpdGlvbi1hbGxcIlxuICAgICAgICA+XG4gICAgICAgICAgPERvd25sb2FkIGNsYXNzTmFtZT1cInctNCBoLTQgc3Ryb2tlLVsyLjJdXCIgLz5cbiAgICAgICAgICA8c3Bhbj5U4bqjaSBk4buxIMOhbiAoLnppcCk8L3NwYW4+XG4gICAgICAgIDwvYnV0dG9uPlxuXG4gICAgICAgIHsvKiBBZGQgVW5pdCBCdXR0b24gKi99XG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXtvbk9wZW5BZGRNb2RhbH1cbiAgICAgICAgICBjbGFzc05hbWU9XCJiZy1lbWVyYWxkLTYwMCBob3ZlcjpiZy1lbWVyYWxkLTUwMCBhY3RpdmU6c2NhbGUtOTUgdGV4dC13aGl0ZSB0ZXh0LXhzIGZvbnQtc2VtaWJvbGQgcHgtMy41IHB5LTIgcm91bmRlZCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41IHNoYWRvdyB0cmFuc2l0aW9uLWFsbFwiXG4gICAgICAgID5cbiAgICAgICAgICA8UGx1cyBjbGFzc05hbWU9XCJ3LTQgaC00IHN0cm9rZS1bMi41XVwiIC8+XG4gICAgICAgICAgPHNwYW4+VGjDqm0gxJHGoW4gduG7izwvc3Bhbj5cbiAgICAgICAgPC9idXR0b24+XG4gICAgICA8L2Rpdj5cbiAgICA8L2hlYWRlcj5cbiAgKTtcbn07XG4iXX0=