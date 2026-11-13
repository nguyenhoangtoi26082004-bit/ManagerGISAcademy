const React = __vite__cjsImport0_react;const _jsxDEV = __vite__cjsImport2_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { LayoutDashboard, Map, Search, Database, Crosshair, BarChart3, CheckCircle2, AlertTriangle } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/Sidebar.tsx";
import __vite__cjsImport2_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const Sidebar = ({ activeTab, onSelectTab, unitsCount, issuesCount }) => {
	const menuItems = [
		{
			id: "overview",
			label: "Tổng quan",
			icon: LayoutDashboard,
			badge: null
		},
		{
			id: "map",
			label: "Bản đồ GIS",
			icon: Map,
			badge: `${unitsCount} vị trí`
		},
		{
			id: "search",
			label: "Tra cứu đơn vị",
			icon: Search,
			badge: null
		},
		{
			id: "management",
			label: "Quản lý dữ liệu",
			icon: Database,
			badge: null
		},
		{
			id: "spatial",
			label: "Phân tích không gian",
			icon: Crosshair,
			badge: "Buffer / K-NN"
		},
		{
			id: "statistics",
			label: "Thống kê và báo cáo",
			icon: BarChart3,
			badge: null
		},
		{
			id: "checker",
			label: "Kiểm tra dữ liệu",
			icon: issuesCount > 0 ? AlertTriangle : CheckCircle2,
			badge: issuesCount > 0 ? `${issuesCount} cảnh báo` : "Chuẩn GIS",
			badgeColor: issuesCount > 0 ? "bg-amber-900/60 text-amber-300" : "bg-emerald-950 text-emerald-300"
		}
	];
	return /* @__PURE__ */ _jsxDEV("aside", {
		className: "w-56 bg-[#0a101d] border-r border-[#1e293b] flex flex-col flex-shrink-0 select-none",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "px-4 pt-4 pb-3 border-b border-[#162032]",
				children: [/* @__PURE__ */ _jsxDEV("h2", {
					className: "text-[11px] font-bold tracking-wider uppercase text-emerald-400",
					children: "CHỨC NĂNG"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 85,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("p", {
					className: "text-xs text-slate-400 font-medium mt-0.5",
					children: "Hệ thống GIS Hà Nội"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 88,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 84,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("nav", {
				className: "p-2 space-y-1.5 flex-1 overflow-y-auto",
				children: menuItems.map((item) => {
					const Icon = item.icon;
					const isActive = activeTab === item.id;
					return /* @__PURE__ */ _jsxDEV("button", {
						onClick: () => onSelectTab(item.id),
						className: `w-full text-left px-3.5 py-2.5 rounded text-xs font-semibold flex items-center justify-between transition-all ${isActive ? "bg-[#059669] text-white shadow-md" : "text-slate-300 hover:bg-[#162032] hover:text-white"}`,
						children: [/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center gap-2.5",
							children: [/* @__PURE__ */ _jsxDEV(Icon, { className: `w-4 h-4 ${isActive ? "text-white" : "text-slate-400"}` }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 108,
								columnNumber: 17
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: item.label }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 109,
								columnNumber: 17
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 107,
							columnNumber: 15
						}, this), item.badge && /* @__PURE__ */ _jsxDEV("span", {
							className: `text-[10px] px-1.5 py-0.5 rounded font-mono ${isActive ? "bg-emerald-800 text-emerald-100" : item.badgeColor || "bg-slate-800 text-slate-400"}`,
							children: item.badge
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 112,
							columnNumber: 17
						}, this)]
					}, item.id, true, {
						fileName: _jsxFileName,
						lineNumber: 98,
						columnNumber: 13
					}, this);
				})
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 92,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "p-3 border-t border-[#162032] bg-[#070c16] text-[10px] text-slate-400 space-y-1",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex justify-between items-center",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-slate-400 font-semibold",
							children: "Cơ sở dữ liệu:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 130,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-emerald-400 font-mono",
							children: "PostGIS / SpatiaLite"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 131,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 129,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex justify-between items-center",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-slate-400 font-semibold",
							children: "Chuẩn bản đồ:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 134,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-sky-400 font-mono",
							children: "VN-2000 / EPSG:32648"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 135,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 133,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex justify-between items-center",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-slate-400 font-semibold",
							children: "Phạm vi:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 138,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-slate-300",
							children: "30 Quận / Huyện Hà Nội"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 139,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 137,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 128,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 82,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,eyJtYXBwaW5ncyI6IkFBQUEsT0FBTyxXQUFXO0FBQ2xCLFNBQ0UsaUJBQ0EsS0FDQSxRQUNBLFVBQ0EsV0FDQSxXQUNBLGNBQ0EscUJBQ0s7OztBQWtCUCxPQUFPLE1BQU0sV0FBbUMsRUFDOUMsV0FDQSxhQUNBLFlBQ0Esa0JBQ0k7Q0FDSixNQUFNLFlBQVk7RUFDaEI7R0FDRSxJQUFJO0dBQ0osT0FBTztHQUNQLE1BQU07R0FDTixPQUFPO0VBQ1Q7RUFDQTtHQUNFLElBQUk7R0FDSixPQUFPO0dBQ1AsTUFBTTtHQUNOLE9BQU8sR0FBRyxXQUFXO0VBQ3ZCO0VBQ0E7R0FDRSxJQUFJO0dBQ0osT0FBTztHQUNQLE1BQU07R0FDTixPQUFPO0VBQ1Q7RUFDQTtHQUNFLElBQUk7R0FDSixPQUFPO0dBQ1AsTUFBTTtHQUNOLE9BQU87RUFDVDtFQUNBO0dBQ0UsSUFBSTtHQUNKLE9BQU87R0FDUCxNQUFNO0dBQ04sT0FBTztFQUNUO0VBQ0E7R0FDRSxJQUFJO0dBQ0osT0FBTztHQUNQLE1BQU07R0FDTixPQUFPO0VBQ1Q7RUFDQTtHQUNFLElBQUk7R0FDSixPQUFPO0dBQ1AsTUFBTSxjQUFjLElBQUksZ0JBQWdCO0dBQ3hDLE9BQU8sY0FBYyxJQUFJLEdBQUcsWUFBWSxhQUFhO0dBQ3JELFlBQVksY0FBYyxJQUFJLG1DQUFtQztFQUNuRTtDQUNGO0NBRUEsT0FDRSx3QkFBQyxTQUFEO0VBQU8sV0FBVTtZQUFqQjtHQUVFLHdCQUFDLE9BQUQ7SUFBSyxXQUFVO2NBQWYsQ0FDRSx3QkFBQyxNQUFEO0tBQUksV0FBVTtlQUFrRTtJQUU1RTs7OztjQUNKLHdCQUFDLEtBQUQ7S0FBRyxXQUFVO2VBQTRDO0lBQXNCOzs7O1lBQzVFOzs7Ozs7R0FHTCx3QkFBQyxPQUFEO0lBQUssV0FBVTtjQUNaLFVBQVUsS0FBSyxTQUFTO0tBQ3ZCLE1BQU0sT0FBTyxLQUFLO0tBQ2xCLE1BQU0sV0FBVyxjQUFjLEtBQUs7S0FFcEMsT0FDRSx3QkFBQyxVQUFEO01BRUUsZUFBZSxZQUFZLEtBQUssRUFBRTtNQUNsQyxXQUFXLGlIQUNULFdBQ0ksc0NBQ0E7Z0JBTlIsQ0FTRSx3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBZixDQUNFLHdCQUFDLE1BQUQsRUFBTSxXQUFXLFdBQVcsV0FBVyxlQUFlLG1CQUFxQjs7OztpQkFDM0Usd0JBQUMsUUFBRCxZQUFPLEtBQUssTUFBWTs7OztlQUNyQjs7Ozs7Z0JBQ0osS0FBSyxTQUNKLHdCQUFDLFFBQUQ7T0FDRSxXQUFXLCtDQUNULFdBQ0ksb0NBQ0EsS0FBSyxjQUFjO2lCQUd4QixLQUFLO01BQ0Y7Ozs7Y0FFRjtRQXZCRCxLQUFLOzs7O1lBdUJKO0lBRVosQ0FBQztHQUNFOzs7OztHQUdMLHdCQUFDLE9BQUQ7SUFBSyxXQUFVO2NBQWY7S0FDRSx3QkFBQyxPQUFEO01BQUssV0FBVTtnQkFBZixDQUNFLHdCQUFDLFFBQUQ7T0FBTSxXQUFVO2lCQUErQjtNQUFvQjs7OztnQkFDbkUsd0JBQUMsUUFBRDtPQUFNLFdBQVU7aUJBQTZCO01BQTBCOzs7O2NBQ3BFOzs7Ozs7S0FDTCx3QkFBQyxPQUFEO01BQUssV0FBVTtnQkFBZixDQUNFLHdCQUFDLFFBQUQ7T0FBTSxXQUFVO2lCQUErQjtNQUFtQjs7OztnQkFDbEUsd0JBQUMsUUFBRDtPQUFNLFdBQVU7aUJBQXlCO01BQTBCOzs7O2NBQ2hFOzs7Ozs7S0FDTCx3QkFBQyxPQUFEO01BQUssV0FBVTtnQkFBZixDQUNFLHdCQUFDLFFBQUQ7T0FBTSxXQUFVO2lCQUErQjtNQUFjOzs7O2dCQUM3RCx3QkFBQyxRQUFEO09BQU0sV0FBVTtpQkFBaUI7TUFBNEI7Ozs7Y0FDMUQ7Ozs7OztJQUNGOzs7Ozs7RUFDQTs7Ozs7O0FBRVgiLCJuYW1lcyI6W10sInNvdXJjZXMiOlsiU2lkZWJhci50c3giXSwidmVyc2lvbiI6Mywic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IFJlYWN0IGZyb20gJ3JlYWN0JztcbmltcG9ydCB7XG4gIExheW91dERhc2hib2FyZCxcbiAgTWFwLFxuICBTZWFyY2gsXG4gIERhdGFiYXNlLFxuICBDcm9zc2hhaXIsXG4gIEJhckNoYXJ0MyxcbiAgQ2hlY2tDaXJjbGUyLFxuICBBbGVydFRyaWFuZ2xlLFxufSBmcm9tICdsdWNpZGUtcmVhY3QnO1xuXG5leHBvcnQgdHlwZSBUYWJUeXBlID1cbiAgfCAnb3ZlcnZpZXcnXG4gIHwgJ21hcCdcbiAgfCAnc2VhcmNoJ1xuICB8ICdtYW5hZ2VtZW50J1xuICB8ICdzcGF0aWFsJ1xuICB8ICdzdGF0aXN0aWNzJ1xuICB8ICdjaGVja2VyJztcblxuaW50ZXJmYWNlIFNpZGViYXJQcm9wcyB7XG4gIGFjdGl2ZVRhYjogVGFiVHlwZTtcbiAgb25TZWxlY3RUYWI6ICh0YWI6IFRhYlR5cGUpID0+IHZvaWQ7XG4gIHVuaXRzQ291bnQ6IG51bWJlcjtcbiAgaXNzdWVzQ291bnQ6IG51bWJlcjtcbn1cblxuZXhwb3J0IGNvbnN0IFNpZGViYXI6IFJlYWN0LkZDPFNpZGViYXJQcm9wcz4gPSAoe1xuICBhY3RpdmVUYWIsXG4gIG9uU2VsZWN0VGFiLFxuICB1bml0c0NvdW50LFxuICBpc3N1ZXNDb3VudCxcbn0pID0+IHtcbiAgY29uc3QgbWVudUl0ZW1zID0gW1xuICAgIHtcbiAgICAgIGlkOiAnb3ZlcnZpZXcnIGFzIFRhYlR5cGUsXG4gICAgICBsYWJlbDogJ1Thu5VuZyBxdWFuJyxcbiAgICAgIGljb246IExheW91dERhc2hib2FyZCxcbiAgICAgIGJhZGdlOiBudWxsLFxuICAgIH0sXG4gICAge1xuICAgICAgaWQ6ICdtYXAnIGFzIFRhYlR5cGUsXG4gICAgICBsYWJlbDogJ0LhuqNuIMSR4buTIEdJUycsXG4gICAgICBpY29uOiBNYXAsXG4gICAgICBiYWRnZTogYCR7dW5pdHNDb3VudH0gduG7iyB0csOtYCxcbiAgICB9LFxuICAgIHtcbiAgICAgIGlkOiAnc2VhcmNoJyBhcyBUYWJUeXBlLFxuICAgICAgbGFiZWw6ICdUcmEgY+G7qXUgxJHGoW4gduG7iycsXG4gICAgICBpY29uOiBTZWFyY2gsXG4gICAgICBiYWRnZTogbnVsbCxcbiAgICB9LFxuICAgIHtcbiAgICAgIGlkOiAnbWFuYWdlbWVudCcgYXMgVGFiVHlwZSxcbiAgICAgIGxhYmVsOiAnUXXhuqNuIGzDvSBk4buvIGxp4buHdScsXG4gICAgICBpY29uOiBEYXRhYmFzZSxcbiAgICAgIGJhZGdlOiBudWxsLFxuICAgIH0sXG4gICAge1xuICAgICAgaWQ6ICdzcGF0aWFsJyBhcyBUYWJUeXBlLFxuICAgICAgbGFiZWw6ICdQaMOibiB0w61jaCBraMO0bmcgZ2lhbicsXG4gICAgICBpY29uOiBDcm9zc2hhaXIsXG4gICAgICBiYWRnZTogJ0J1ZmZlciAvIEstTk4nLFxuICAgIH0sXG4gICAge1xuICAgICAgaWQ6ICdzdGF0aXN0aWNzJyBhcyBUYWJUeXBlLFxuICAgICAgbGFiZWw6ICdUaOG7kW5nIGvDqiB2w6AgYsOhbyBjw6FvJyxcbiAgICAgIGljb246IEJhckNoYXJ0MyxcbiAgICAgIGJhZGdlOiBudWxsLFxuICAgIH0sXG4gICAge1xuICAgICAgaWQ6ICdjaGVja2VyJyBhcyBUYWJUeXBlLFxuICAgICAgbGFiZWw6ICdLaeG7g20gdHJhIGThu68gbGnhu4d1JyxcbiAgICAgIGljb246IGlzc3Vlc0NvdW50ID4gMCA/IEFsZXJ0VHJpYW5nbGUgOiBDaGVja0NpcmNsZTIsXG4gICAgICBiYWRnZTogaXNzdWVzQ291bnQgPiAwID8gYCR7aXNzdWVzQ291bnR9IGPhuqNuaCBiw6FvYCA6ICdDaHXhuqluIEdJUycsXG4gICAgICBiYWRnZUNvbG9yOiBpc3N1ZXNDb3VudCA+IDAgPyAnYmctYW1iZXItOTAwLzYwIHRleHQtYW1iZXItMzAwJyA6ICdiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwJyxcbiAgICB9LFxuICBdO1xuXG4gIHJldHVybiAoXG4gICAgPGFzaWRlIGNsYXNzTmFtZT1cInctNTYgYmctWyMwYTEwMWRdIGJvcmRlci1yIGJvcmRlci1bIzFlMjkzYl0gZmxleCBmbGV4LWNvbCBmbGV4LXNocmluay0wIHNlbGVjdC1ub25lXCI+XG4gICAgICB7LyogQ2F0ZWdvcnkgVGl0bGUgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cInB4LTQgcHQtNCBwYi0zIGJvcmRlci1iIGJvcmRlci1bIzE2MjAzMl1cIj5cbiAgICAgICAgPGgyIGNsYXNzTmFtZT1cInRleHQtWzExcHhdIGZvbnQtYm9sZCB0cmFja2luZy13aWRlciB1cHBlcmNhc2UgdGV4dC1lbWVyYWxkLTQwMFwiPlxuICAgICAgICAgIENI4buoQyBOxIJOR1xuICAgICAgICA8L2gyPlxuICAgICAgICA8cCBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGZvbnQtbWVkaXVtIG10LTAuNVwiPkjhu4cgdGjhu5FuZyBHSVMgSMOgIE7hu5lpPC9wPlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBNZW51IExpc3QgKi99XG4gICAgICA8bmF2IGNsYXNzTmFtZT1cInAtMiBzcGFjZS15LTEuNSBmbGV4LTEgb3ZlcmZsb3cteS1hdXRvXCI+XG4gICAgICAgIHttZW51SXRlbXMubWFwKChpdGVtKSA9PiB7XG4gICAgICAgICAgY29uc3QgSWNvbiA9IGl0ZW0uaWNvbjtcbiAgICAgICAgICBjb25zdCBpc0FjdGl2ZSA9IGFjdGl2ZVRhYiA9PT0gaXRlbS5pZDtcblxuICAgICAgICAgIHJldHVybiAoXG4gICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgIGtleT17aXRlbS5pZH1cbiAgICAgICAgICAgICAgb25DbGljaz17KCkgPT4gb25TZWxlY3RUYWIoaXRlbS5pZCl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT17YHctZnVsbCB0ZXh0LWxlZnQgcHgtMy41IHB5LTIuNSByb3VuZGVkIHRleHQteHMgZm9udC1zZW1pYm9sZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gdHJhbnNpdGlvbi1hbGwgJHtcbiAgICAgICAgICAgICAgICBpc0FjdGl2ZVxuICAgICAgICAgICAgICAgICAgPyAnYmctWyMwNTk2NjldIHRleHQtd2hpdGUgc2hhZG93LW1kJ1xuICAgICAgICAgICAgICAgICAgOiAndGV4dC1zbGF0ZS0zMDAgaG92ZXI6YmctWyMxNjIwMzJdIGhvdmVyOnRleHQtd2hpdGUnXG4gICAgICAgICAgICAgIH1gfVxuICAgICAgICAgICAgPlxuICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGdhcC0yLjVcIj5cbiAgICAgICAgICAgICAgICA8SWNvbiBjbGFzc05hbWU9e2B3LTQgaC00ICR7aXNBY3RpdmUgPyAndGV4dC13aGl0ZScgOiAndGV4dC1zbGF0ZS00MDAnfWB9IC8+XG4gICAgICAgICAgICAgICAgPHNwYW4+e2l0ZW0ubGFiZWx9PC9zcGFuPlxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAge2l0ZW0uYmFkZ2UgJiYgKFxuICAgICAgICAgICAgICAgIDxzcGFuXG4gICAgICAgICAgICAgICAgICBjbGFzc05hbWU9e2B0ZXh0LVsxMHB4XSBweC0xLjUgcHktMC41IHJvdW5kZWQgZm9udC1tb25vICR7XG4gICAgICAgICAgICAgICAgICAgIGlzQWN0aXZlXG4gICAgICAgICAgICAgICAgICAgICAgPyAnYmctZW1lcmFsZC04MDAgdGV4dC1lbWVyYWxkLTEwMCdcbiAgICAgICAgICAgICAgICAgICAgICA6IGl0ZW0uYmFkZ2VDb2xvciB8fCAnYmctc2xhdGUtODAwIHRleHQtc2xhdGUtNDAwJ1xuICAgICAgICAgICAgICAgICAgfWB9XG4gICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAge2l0ZW0uYmFkZ2V9XG4gICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICApfVxuICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgKTtcbiAgICAgICAgfSl9XG4gICAgICA8L25hdj5cblxuICAgICAgey8qIE1pbGl0YXJ5IEdJUyBUZWNoIFRhZyAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC0zIGJvcmRlci10IGJvcmRlci1bIzE2MjAzMl0gYmctWyMwNzBjMTZdIHRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHNwYWNlLXktMVwiPlxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXgganVzdGlmeS1iZXR3ZWVuIGl0ZW1zLWNlbnRlclwiPlxuICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwIGZvbnQtc2VtaWJvbGRcIj5DxqEgc+G7nyBk4buvIGxp4buHdTo8L3NwYW4+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1lbWVyYWxkLTQwMCBmb250LW1vbm9cIj5Qb3N0R0lTIC8gU3BhdGlhTGl0ZTwvc3Bhbj5cbiAgICAgICAgPC9kaXY+XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gaXRlbXMtY2VudGVyXCI+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDAgZm9udC1zZW1pYm9sZFwiPkNodeG6qW4gYuG6o24gxJHhu5M6PC9zcGFuPlxuICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2t5LTQwMCBmb250LW1vbm9cIj5WTi0yMDAwIC8gRVBTRzozMjY0ODwvc3Bhbj5cbiAgICAgICAgPC9kaXY+XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gaXRlbXMtY2VudGVyXCI+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDAgZm9udC1zZW1pYm9sZFwiPlBo4bqhbSB2aTo8L3NwYW4+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS0zMDBcIj4zMCBRdeG6rW4gLyBIdXnhu4duIEjDoCBO4buZaTwvc3Bhbj5cbiAgICAgICAgPC9kaXY+XG4gICAgICA8L2Rpdj5cbiAgICA8L2FzaWRlPlxuICApO1xufTtcbiJdfQ==