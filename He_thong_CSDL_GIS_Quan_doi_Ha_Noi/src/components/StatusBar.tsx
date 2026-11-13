const React = __vite__cjsImport0_react;const _jsxDEV = __vite__cjsImport2_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { Layers, Crosshair, Globe } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/StatusBar.tsx";
import __vite__cjsImport2_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const StatusBar = ({ cursorCoord, activeLayerName, totalUnitsCount, basemapName }) => {
	return /* @__PURE__ */ _jsxDEV("footer", {
		className: "h-7 bg-[#050811] border-t border-[#1e293b] text-slate-400 text-[11px] flex items-center justify-between px-3 select-none z-30 font-mono",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex items-center space-x-3 overflow-hidden text-ellipsis whitespace-nowrap",
				children: [
					/* @__PURE__ */ _jsxDEV("span", {
						className: "flex items-center gap-1.5 text-emerald-400",
						children: [/* @__PURE__ */ _jsxDEV("span", { className: "w-1.5 h-1.5 rounded-full bg-emerald-400 animate-ping" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 22,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "font-semibold",
							children: "Sẵn sàng"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 23,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 21,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", {
						className: "text-slate-600",
						children: "|"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 25,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", { children: [
						"TP. Hà Nội (",
						totalUnitsCount,
						" đơn vị)"
					] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 26,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", {
						className: "text-slate-600",
						children: "|"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 27,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", { children: "EPSG:32648" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 28,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", {
						className: "text-slate-600",
						children: "|"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 29,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", {
						className: "hidden sm:inline",
						children: ["Nền: ", basemapName]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 30,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 20,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "hidden md:flex items-center space-x-2 text-slate-300",
				children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-3 h-3 text-emerald-500" }, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 35,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("span", { children: ["Lớp: ", activeLayerName || "Tất cả đơn vị quân đội"] }, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 36,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 34,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex items-center space-x-4",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center space-x-2 text-slate-200",
						children: [/* @__PURE__ */ _jsxDEV(Crosshair, { className: "w-3 h-3 text-sky-400" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 43,
							columnNumber: 11
						}, this), cursorCoord ? /* @__PURE__ */ _jsxDEV("span", { children: [
							"X: ",
							/* @__PURE__ */ _jsxDEV("strong", {
								className: "text-emerald-400",
								children: cursorCoord.x.toLocaleString()
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 46,
								columnNumber: 18
							}, this),
							"m, Y:",
							" ",
							/* @__PURE__ */ _jsxDEV("strong", {
								className: "text-emerald-400",
								children: cursorCoord.y.toLocaleString()
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 47,
								columnNumber: 15
							}, this),
							"m (",
							cursorCoord.lat.toFixed(4),
							"°N, ",
							cursorCoord.lng.toFixed(4),
							"°E)"
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 45,
							columnNumber: 13
						}, this) : /* @__PURE__ */ _jsxDEV("span", {
							className: "text-slate-400",
							children: "X/Y (m): Rê chuột trên bản đồ"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 51,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 42,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", {
						className: "text-slate-600",
						children: "|"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 55,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center space-x-1.5 text-emerald-400 font-semibold",
						children: [/* @__PURE__ */ _jsxDEV(Globe, { className: "w-3 h-3" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 59,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "CRS: EPSG:32648 | mét" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 60,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 58,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 40,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 18,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,eyJtYXBwaW5ncyI6IkFBQUEsT0FBTyxXQUFXO0FBQ2xCLFNBQVMsUUFBUSxXQUFXLGFBQXdCOzs7QUFTcEQsT0FBTyxNQUFNLGFBQXVDLEVBQ2xELGFBQ0EsaUJBQ0EsaUJBQ0Esa0JBQ0k7Q0FDSixPQUNFLHdCQUFDLFVBQUQ7RUFBUSxXQUFVO1lBQWxCO0dBRUUsd0JBQUMsT0FBRDtJQUFLLFdBQVU7Y0FBZjtLQUNFLHdCQUFDLFFBQUQ7TUFBTSxXQUFVO2dCQUFoQixDQUNFLHdCQUFDLFFBQUQsRUFBTSxXQUFVLHVEQUE2RDs7OztnQkFDN0Usd0JBQUMsUUFBRDtPQUFNLFdBQVU7aUJBQWdCO01BQWM7Ozs7Y0FDMUM7Ozs7OztLQUNOLHdCQUFDLFFBQUQ7TUFBTSxXQUFVO2dCQUFpQjtLQUFPOzs7OztLQUN4Qyx3QkFBQyxRQUFEO01BQU07TUFBYTtNQUFnQjtLQUFjOzs7OztLQUNqRCx3QkFBQyxRQUFEO01BQU0sV0FBVTtnQkFBaUI7S0FBTzs7Ozs7S0FDeEMsd0JBQUMsUUFBRCxZQUFNLGFBQWdCOzs7OztLQUN0Qix3QkFBQyxRQUFEO01BQU0sV0FBVTtnQkFBaUI7S0FBTzs7Ozs7S0FDeEMsd0JBQUMsUUFBRDtNQUFNLFdBQVU7Z0JBQWhCLENBQW1DLFNBQU0sV0FBa0I7Ozs7OztJQUN4RDs7Ozs7O0dBR0wsd0JBQUMsT0FBRDtJQUFLLFdBQVU7Y0FBZixDQUNFLHdCQUFDLFFBQUQsRUFBUSxXQUFVLDJCQUE0Qjs7OztjQUM5Qyx3QkFBQyxRQUFELGFBQU0sU0FBTSxtQkFBbUIsd0JBQStCOzs7O1lBQzNEOzs7Ozs7R0FHTCx3QkFBQyxPQUFEO0lBQUssV0FBVTtjQUFmO0tBRUUsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxXQUFELEVBQVcsV0FBVSx1QkFBd0I7Ozs7Z0JBQzVDLGNBQ0Msd0JBQUMsUUFBRDtPQUFNO09BQ0Qsd0JBQUMsVUFBRDtRQUFRLFdBQVU7a0JBQW9CLFlBQVksRUFBRSxlQUFlO09BQVU7Ozs7O09BQUM7T0FBTTtPQUN2Rix3QkFBQyxVQUFEO1FBQVEsV0FBVTtrQkFBb0IsWUFBWSxFQUFFLGVBQWU7T0FBVTs7Ozs7T0FBQztPQUM3RSxZQUFZLElBQUksUUFBUSxDQUFDO09BQUU7T0FBSyxZQUFZLElBQUksUUFBUSxDQUFDO09BQUU7TUFDeEQ7Ozs7aUJBRU4sd0JBQUMsUUFBRDtPQUFNLFdBQVU7aUJBQWlCO01BQW1DOzs7O2NBRW5FOzs7Ozs7S0FFTCx3QkFBQyxRQUFEO01BQU0sV0FBVTtnQkFBaUI7S0FBTzs7Ozs7S0FHeEMsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxPQUFELEVBQU8sV0FBVSxVQUFXOzs7O2dCQUM1Qix3QkFBQyxRQUFELFlBQU0sd0JBQTJCOzs7O2NBQzlCOzs7Ozs7SUFDRjs7Ozs7O0VBQ0M7Ozs7OztBQUVaIiwibmFtZXMiOltdLCJzb3VyY2VzIjpbIlN0YXR1c0Jhci50c3giXSwidmVyc2lvbiI6Mywic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IFJlYWN0IGZyb20gJ3JlYWN0JztcbmltcG9ydCB7IExheWVycywgQ3Jvc3NoYWlyLCBHbG9iZSwgSGFyZERyaXZlIH0gZnJvbSAnbHVjaWRlLXJlYWN0JztcblxuaW50ZXJmYWNlIFN0YXR1c0JhclByb3BzIHtcbiAgY3Vyc29yQ29vcmQ6IHsgbGF0OiBudW1iZXI7IGxuZzogbnVtYmVyOyB4OiBudW1iZXI7IHk6IG51bWJlciB9IHwgbnVsbDtcbiAgYWN0aXZlTGF5ZXJOYW1lOiBzdHJpbmc7XG4gIHRvdGFsVW5pdHNDb3VudDogbnVtYmVyO1xuICBiYXNlbWFwTmFtZTogc3RyaW5nO1xufVxuXG5leHBvcnQgY29uc3QgU3RhdHVzQmFyOiBSZWFjdC5GQzxTdGF0dXNCYXJQcm9wcz4gPSAoe1xuICBjdXJzb3JDb29yZCxcbiAgYWN0aXZlTGF5ZXJOYW1lLFxuICB0b3RhbFVuaXRzQ291bnQsXG4gIGJhc2VtYXBOYW1lLFxufSkgPT4ge1xuICByZXR1cm4gKFxuICAgIDxmb290ZXIgY2xhc3NOYW1lPVwiaC03IGJnLVsjMDUwODExXSBib3JkZXItdCBib3JkZXItWyMxZTI5M2JdIHRleHQtc2xhdGUtNDAwIHRleHQtWzExcHhdIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBweC0zIHNlbGVjdC1ub25lIHotMzAgZm9udC1tb25vXCI+XG4gICAgICB7LyogTGVmdCBzdGF0dXMgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIHNwYWNlLXgtMyBvdmVyZmxvdy1oaWRkZW4gdGV4dC1lbGxpcHNpcyB3aGl0ZXNwYWNlLW5vd3JhcFwiPlxuICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41IHRleHQtZW1lcmFsZC00MDBcIj5cbiAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ3LTEuNSBoLTEuNSByb3VuZGVkLWZ1bGwgYmctZW1lcmFsZC00MDAgYW5pbWF0ZS1waW5nXCI+PC9zcGFuPlxuICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cImZvbnQtc2VtaWJvbGRcIj5T4bq1biBzw6BuZzwvc3Bhbj5cbiAgICAgICAgPC9zcGFuPlxuICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTYwMFwiPnw8L3NwYW4+XG4gICAgICAgIDxzcGFuPlRQLiBIw6AgTuG7mWkgKHt0b3RhbFVuaXRzQ291bnR9IMSRxqFuIHbhu4spPC9zcGFuPlxuICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTYwMFwiPnw8L3NwYW4+XG4gICAgICAgIDxzcGFuPkVQU0c6MzI2NDg8L3NwYW4+XG4gICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNjAwXCI+fDwvc3Bhbj5cbiAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwiaGlkZGVuIHNtOmlubGluZVwiPk7hu4FuOiB7YmFzZW1hcE5hbWV9PC9zcGFuPlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBDZW50ZXIgLSBBY3RpdmUgTGF5ZXIgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImhpZGRlbiBtZDpmbGV4IGl0ZW1zLWNlbnRlciBzcGFjZS14LTIgdGV4dC1zbGF0ZS0zMDBcIj5cbiAgICAgICAgPExheWVycyBjbGFzc05hbWU9XCJ3LTMgaC0zIHRleHQtZW1lcmFsZC01MDBcIiAvPlxuICAgICAgICA8c3Bhbj5M4bubcDoge2FjdGl2ZUxheWVyTmFtZSB8fCAnVOG6pXQgY+G6oyDEkcahbiB24buLIHF1w6JuIMSR4buZaSd9PC9zcGFuPlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBSaWdodCAtIExpdmUgQ29vcmRpbmF0ZXMgJiBQcm9qZWN0aW9uICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBzcGFjZS14LTRcIj5cbiAgICAgICAgey8qIENvb3JkaW5hdGVzICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIHNwYWNlLXgtMiB0ZXh0LXNsYXRlLTIwMFwiPlxuICAgICAgICAgIDxDcm9zc2hhaXIgY2xhc3NOYW1lPVwidy0zIGgtMyB0ZXh0LXNreS00MDBcIiAvPlxuICAgICAgICAgIHtjdXJzb3JDb29yZCA/IChcbiAgICAgICAgICAgIDxzcGFuPlxuICAgICAgICAgICAgICBYOiA8c3Ryb25nIGNsYXNzTmFtZT1cInRleHQtZW1lcmFsZC00MDBcIj57Y3Vyc29yQ29vcmQueC50b0xvY2FsZVN0cmluZygpfTwvc3Ryb25nPm0sIFk6eycgJ31cbiAgICAgICAgICAgICAgPHN0cm9uZyBjbGFzc05hbWU9XCJ0ZXh0LWVtZXJhbGQtNDAwXCI+e2N1cnNvckNvb3JkLnkudG9Mb2NhbGVTdHJpbmcoKX08L3N0cm9uZz5tIChcbiAgICAgICAgICAgICAge2N1cnNvckNvb3JkLmxhdC50b0ZpeGVkKDQpfcKwTiwge2N1cnNvckNvb3JkLmxuZy50b0ZpeGVkKDQpfcKwRSlcbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICApIDogKFxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDBcIj5YL1kgKG0pOiBSw6ogY2h14buZdCB0csOqbiBi4bqjbiDEkeG7kzwvc3Bhbj5cbiAgICAgICAgICApfVxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTYwMFwiPnw8L3NwYW4+XG5cbiAgICAgICAgey8qIENSUyAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBzcGFjZS14LTEuNSB0ZXh0LWVtZXJhbGQtNDAwIGZvbnQtc2VtaWJvbGRcIj5cbiAgICAgICAgICA8R2xvYmUgY2xhc3NOYW1lPVwidy0zIGgtM1wiIC8+XG4gICAgICAgICAgPHNwYW4+Q1JTOiBFUFNHOjMyNjQ4IHwgbcOpdDwvc3Bhbj5cbiAgICAgICAgPC9kaXY+XG4gICAgICA8L2Rpdj5cbiAgICA8L2Zvb3Rlcj5cbiAgKTtcbn07XG4iXX0=