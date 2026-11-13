const React = __vite__cjsImport0_react;const _jsxDEV = __vite__cjsImport2_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { X, MapPin, Phone, Shield, Activity, Award, Compass, Layers, CircleDot, Edit } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/UnitDetailDrawer.tsx";
import __vite__cjsImport2_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const UnitDetailDrawer = ({ unit, onClose, onEdit, onTriggerBuffer, onCenterMap }) => {
	if (!unit) return null;
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "absolute top-3 right-3 bottom-10 z-[1002] w-96 bg-[#09101e]/98 backdrop-blur-xl border border-slate-700/80 rounded-xl shadow-2xl flex flex-col text-white overflow-hidden animate-in slide-in-from-right duration-300",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "p-4 bg-gradient-to-r from-slate-900 via-slate-800 to-slate-900 border-b border-slate-700/80 relative",
				children: [
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: onClose,
						className: "absolute top-3 right-3 text-slate-400 hover:text-white p-1 rounded-full hover:bg-slate-700 transition",
						children: /* @__PURE__ */ _jsxDEV(X, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 44,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 40,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center gap-2 mb-1",
						children: [
							/* @__PURE__ */ _jsxDEV("span", {
								className: "font-mono text-xs px-2 py-0.5 rounded bg-emerald-950 text-emerald-300 border border-emerald-700/70 font-semibold",
								children: unit.code
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 48,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[11px] px-2 py-0.5 rounded bg-amber-950 text-amber-300 border border-amber-700/50",
								children: unit.securityLevel
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 51,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[11px] px-2 py-0.5 rounded bg-sky-950 text-sky-300 border border-sky-700/50",
								children: unit.unitType
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 54,
								columnNumber: 11
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 47,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("h3", {
						className: "text-base font-bold text-white leading-snug",
						children: unit.name
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 59,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("p", {
						className: "text-xs text-slate-400 mt-0.5",
						children: unit.parentUnit
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 60,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 39,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "p-4 space-y-4 overflow-y-auto flex-1 text-xs",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: `p-2.5 rounded-lg border flex items-center justify-between ${unit.readinessStatus.includes("Cấp 1") ? "bg-rose-950/50 border-rose-700/60 text-rose-200" : "bg-emerald-950/40 border-emerald-700/50 text-emerald-200"}`,
						children: [/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center gap-2",
							children: [/* @__PURE__ */ _jsxDEV(Activity, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 74,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[10px] uppercase font-bold text-slate-400",
								children: "Trạng thái SSCĐ"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 76,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("div", {
								className: "font-bold text-xs",
								children: unit.readinessStatus
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 77,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 75,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 73,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-[10px] font-mono bg-slate-900/60 px-2 py-1 rounded",
							children: "24/7 Sẵn sàng"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 80,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 66,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0f172a] p-3 rounded-lg border border-slate-700/70 space-y-2 font-mono",
						children: [/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center justify-between border-b border-slate-800 pb-1.5 text-[11px] text-emerald-400 font-sans font-bold",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "flex items-center gap-1.5",
								children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-3.5 h-3.5 text-emerald-400" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 89,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", { children: "DỮ LIỆU TỌA ĐỘ GIS" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 90,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 88,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-[10px] text-slate-400",
								children: "EPSG:32648 & 4326"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 92,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 87,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "grid grid-cols-2 gap-2 text-[11px]",
							children: [
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400 text-[10px] block font-sans",
									children: "Tọa độ X (Easting):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 97,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-emerald-300 font-bold",
									children: [unit.x_vn2000.toLocaleString(), " m"]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 98,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 96,
									columnNumber: 13
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400 text-[10px] block font-sans",
									children: "Tọa độ Y (Northing):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 101,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-emerald-300 font-bold",
									children: [unit.y_vn2000.toLocaleString(), " m"]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 102,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 100,
									columnNumber: 13
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400 text-[10px] block font-sans",
									children: "Vĩ độ (WGS84):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 105,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-sky-300",
									children: [unit.latitude.toFixed(6), "°N"]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 106,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 104,
									columnNumber: 13
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400 text-[10px] block font-sans",
									children: "Kinh độ (WGS84):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 109,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-sky-300",
									children: [unit.longitude.toFixed(6), "°E"]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 110,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 108,
									columnNumber: 13
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 95,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 86,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-2 bg-[#0f172a] p-3 rounded-lg border border-slate-700/70",
						children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-start gap-2",
								children: [/* @__PURE__ */ _jsxDEV(Award, { className: "w-4 h-4 text-amber-400 flex-shrink-0 mt-0.5" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 118,
									columnNumber: 13
								}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-[10px] text-slate-400 block",
									children: "Chỉ huy trưởng / Giám đốc:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 120,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("strong", {
									className: "text-white text-xs",
									children: unit.commander || "Đang cập nhật"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 121,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 119,
									columnNumber: 13
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 117,
								columnNumber: 11
							}, this),
							unit.politicalCommissar && /* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-start gap-2 border-t border-slate-800 pt-2",
								children: [/* @__PURE__ */ _jsxDEV(Shield, { className: "w-4 h-4 text-rose-400 flex-shrink-0 mt-0.5" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 127,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-[10px] text-slate-400 block",
									children: "Chính ủy / Chính trị viên:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 129,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("strong", {
									className: "text-white text-xs",
									children: unit.politicalCommissar
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 130,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 128,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 126,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-center gap-2 border-t border-slate-800 pt-2",
								children: [/* @__PURE__ */ _jsxDEV(Phone, { className: "w-4 h-4 text-emerald-400 flex-shrink-0" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 136,
									columnNumber: 13
								}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-[10px] text-slate-400 block",
									children: "Trực ban tác chiến:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 138,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "font-mono text-emerald-300 font-bold",
									children: unit.phone
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 139,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 137,
									columnNumber: 13
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 135,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-start gap-2 border-t border-slate-800 pt-2",
								children: [/* @__PURE__ */ _jsxDEV(MapPin, { className: "w-4 h-4 text-sky-400 flex-shrink-0 mt-0.5" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 144,
									columnNumber: 13
								}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-[10px] text-slate-400 block",
									children: "Địa chỉ đóng quân:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 146,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-200",
									children: unit.address
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 147,
									columnNumber: 15
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 145,
									columnNumber: 13
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 143,
								columnNumber: 11
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 116,
						columnNumber: 9
					}, this),
					unit.mainMissions && unit.mainMissions.length > 0 && /* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-1.5",
						children: [/* @__PURE__ */ _jsxDEV("h4", {
							className: "text-[11px] font-bold text-slate-300 uppercase tracking-wider flex items-center gap-1.5",
							children: [/* @__PURE__ */ _jsxDEV(Shield, { className: "w-3.5 h-3.5 text-emerald-400" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 156,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Nhiệm vụ tác chiến chủ yếu" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 157,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 155,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("ul", {
							className: "space-y-1 pl-4 list-disc text-slate-300 text-[11px]",
							children: unit.mainMissions.map((m, idx) => /* @__PURE__ */ _jsxDEV("li", { children: m }, idx, false, {
								fileName: _jsxFileName,
								lineNumber: 161,
								columnNumber: 17
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 159,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 154,
						columnNumber: 11
					}, this),
					unit.equipmentHighlights && unit.equipmentHighlights.length > 0 && /* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-1.5",
						children: [/* @__PURE__ */ _jsxDEV("h4", {
							className: "text-[11px] font-bold text-slate-300 uppercase tracking-wider flex items-center gap-1.5",
							children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-3.5 h-3.5 text-amber-400" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 171,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Vũ khí, khí tài & trang bị kỹ thuật" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 172,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 170,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "flex flex-wrap gap-1.5 pt-0.5",
							children: unit.equipmentHighlights.map((eq, idx) => /* @__PURE__ */ _jsxDEV("span", {
								className: "px-2 py-0.5 bg-slate-800 text-slate-300 rounded border border-slate-700 text-[10px]",
								children: eq
							}, idx, false, {
								fileName: _jsxFileName,
								lineNumber: 176,
								columnNumber: 17
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 174,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 169,
						columnNumber: 11
					}, this),
					unit.notes && /* @__PURE__ */ _jsxDEV("div", {
						className: "p-2.5 rounded bg-slate-900/60 border border-slate-800 text-[11px] text-slate-400 italic",
						children: [
							"\"",
							unit.notes,
							"\""
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 189,
						columnNumber: 11
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 64,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "p-3 bg-slate-900 border-t border-slate-800 flex items-center justify-between gap-2",
				children: [
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => onCenterMap(unit),
						className: "flex-1 bg-slate-800 hover:bg-slate-700 text-white text-xs font-semibold py-2 px-3 rounded flex items-center justify-center gap-1.5 transition",
						children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-3.5 h-3.5 text-sky-400" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 201,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Định vị" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 202,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 197,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => onTriggerBuffer(unit),
						className: "flex-1 bg-amber-700/80 hover:bg-amber-600 text-white text-xs font-semibold py-2 px-3 rounded flex items-center justify-center gap-1.5 transition",
						children: [/* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-3.5 h-3.5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 208,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Vùng đệm (10km)" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 209,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 204,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => onEdit(unit),
						className: "bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold p-2 rounded transition",
						title: "Sửa thông tin đơn vị",
						children: /* @__PURE__ */ _jsxDEV(Edit, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 216,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 211,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 196,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 37,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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QUFELFlBQU0sa0JBQXFCOzs7O2NBQ3JCOzs7Ozs7S0FDUix3QkFBQyxVQUFEO01BQ0UsZUFBZSxPQUFPLElBQUk7TUFDMUIsV0FBVTtNQUNWLE9BQU07Z0JBRU4sd0JBQUMsTUFBRCxFQUFNLFdBQVUsVUFBVzs7Ozs7S0FDckI7Ozs7O0lBQ0w7Ozs7OztFQUNGOzs7Ozs7QUFFVCIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJVbml0RGV0YWlsRHJhd2VyLnRzeCJdLCJ2ZXJzaW9uIjozLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgUmVhY3QgZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0IH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7XG4gIFgsXG4gIE1hcFBpbixcbiAgUGhvbmUsXG4gIFNoaWVsZCxcbiAgQWN0aXZpdHksXG4gIEF3YXJkLFxuICBVc2VycyxcbiAgQ29tcGFzcyxcbiAgQ2FsZW5kYXIsXG4gIExheWVycyxcbiAgQ2lyY2xlRG90LFxuICBFZGl0LFxuICBFeHRlcm5hbExpbmssXG59IGZyb20gJ2x1Y2lkZS1yZWFjdCc7XG5cbmludGVyZmFjZSBVbml0RGV0YWlsRHJhd2VyUHJvcHMge1xuICB1bml0OiBNaWxpdGFyeVVuaXQgfCBudWxsO1xuICBvbkNsb3NlOiAoKSA9PiB2b2lkO1xuICBvbkVkaXQ6ICh1bml0OiBNaWxpdGFyeVVuaXQpID0+IHZvaWQ7XG4gIG9uVHJpZ2dlckJ1ZmZlcjogKHVuaXQ6IE1pbGl0YXJ5VW5pdCkgPT4gdm9pZDtcbiAgb25DZW50ZXJNYXA6ICh1bml0OiBNaWxpdGFyeVVuaXQpID0+IHZvaWQ7XG59XG5cbmV4cG9ydCBjb25zdCBVbml0RGV0YWlsRHJhd2VyOiBSZWFjdC5GQzxVbml0RGV0YWlsRHJhd2VyUHJvcHM+ID0gKHtcbiAgdW5pdCxcbiAgb25DbG9zZSxcbiAgb25FZGl0LFxuICBvblRyaWdnZXJCdWZmZXIsXG4gIG9uQ2VudGVyTWFwLFxufSkgPT4ge1xuICBpZiAoIXVuaXQpIHJldHVybiBudWxsO1xuXG4gIHJldHVybiAoXG4gICAgPGRpdiBjbGFzc05hbWU9XCJhYnNvbHV0ZSB0b3AtMyByaWdodC0zIGJvdHRvbS0xMCB6LVsxMDAyXSB3LTk2IGJnLVsjMDkxMDFlXS85OCBiYWNrZHJvcC1ibHVyLXhsIGJvcmRlciBib3JkZXItc2xhdGUtNzAwLzgwIHJvdW5kZWQteGwgc2hhZG93LTJ4bCBmbGV4IGZsZXgtY29sIHRleHQtd2hpdGUgb3ZlcmZsb3ctaGlkZGVuIGFuaW1hdGUtaW4gc2xpZGUtaW4tZnJvbS1yaWdodCBkdXJhdGlvbi0zMDBcIj5cbiAgICAgIHsvKiBEcmF3ZXIgSGVhZGVyICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJwLTQgYmctZ3JhZGllbnQtdG8tciBmcm9tLXNsYXRlLTkwMCB2aWEtc2xhdGUtODAwIHRvLXNsYXRlLTkwMCBib3JkZXItYiBib3JkZXItc2xhdGUtNzAwLzgwIHJlbGF0aXZlXCI+XG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXtvbkNsb3NlfVxuICAgICAgICAgIGNsYXNzTmFtZT1cImFic29sdXRlIHRvcC0zIHJpZ2h0LTMgdGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC13aGl0ZSBwLTEgcm91bmRlZC1mdWxsIGhvdmVyOmJnLXNsYXRlLTcwMCB0cmFuc2l0aW9uXCJcbiAgICAgICAgPlxuICAgICAgICAgIDxYIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICA8L2J1dHRvbj5cblxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGdhcC0yIG1iLTFcIj5cbiAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC14cyBweC0yIHB5LTAuNSByb3VuZGVkIGJnLWVtZXJhbGQtOTUwIHRleHQtZW1lcmFsZC0zMDAgYm9yZGVyIGJvcmRlci1lbWVyYWxkLTcwMC83MCBmb250LXNlbWlib2xkXCI+XG4gICAgICAgICAgICB7dW5pdC5jb2RlfVxuICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSBweC0yIHB5LTAuNSByb3VuZGVkIGJnLWFtYmVyLTk1MCB0ZXh0LWFtYmVyLTMwMCBib3JkZXIgYm9yZGVyLWFtYmVyLTcwMC81MFwiPlxuICAgICAgICAgICAge3VuaXQuc2VjdXJpdHlMZXZlbH1cbiAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gcHgtMiBweS0wLjUgcm91bmRlZCBiZy1za3ktOTUwIHRleHQtc2t5LTMwMCBib3JkZXIgYm9yZGVyLXNreS03MDAvNTBcIj5cbiAgICAgICAgICAgIHt1bml0LnVuaXRUeXBlfVxuICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgPGgzIGNsYXNzTmFtZT1cInRleHQtYmFzZSBmb250LWJvbGQgdGV4dC13aGl0ZSBsZWFkaW5nLXNudWdcIj57dW5pdC5uYW1lfTwvaDM+XG4gICAgICAgIDxwIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDAgbXQtMC41XCI+e3VuaXQucGFyZW50VW5pdH08L3A+XG4gICAgICA8L2Rpdj5cblxuICAgICAgey8qIERyYXdlciBCb2R5ICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJwLTQgc3BhY2UteS00IG92ZXJmbG93LXktYXV0byBmbGV4LTEgdGV4dC14c1wiPlxuICAgICAgICB7LyogUmVhZGluZXNzIEFsZXJ0IEJhZGdlICovfVxuICAgICAgICA8ZGl2XG4gICAgICAgICAgY2xhc3NOYW1lPXtgcC0yLjUgcm91bmRlZC1sZyBib3JkZXIgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuICR7XG4gICAgICAgICAgICB1bml0LnJlYWRpbmVzc1N0YXR1cy5pbmNsdWRlcygnQ+G6pXAgMScpXG4gICAgICAgICAgICAgID8gJ2JnLXJvc2UtOTUwLzUwIGJvcmRlci1yb3NlLTcwMC82MCB0ZXh0LXJvc2UtMjAwJ1xuICAgICAgICAgICAgICA6ICdiZy1lbWVyYWxkLTk1MC80MCBib3JkZXItZW1lcmFsZC03MDAvNTAgdGV4dC1lbWVyYWxkLTIwMCdcbiAgICAgICAgICB9YH1cbiAgICAgICAgPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgIDxBY3Rpdml0eSBjbGFzc05hbWU9XCJ3LTQgaC00XCIgLz5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdXBwZXJjYXNlIGZvbnQtYm9sZCB0ZXh0LXNsYXRlLTQwMFwiPlRy4bqhbmcgdGjDoWkgU1NDxJA8L2Rpdj5cbiAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmb250LWJvbGQgdGV4dC14c1wiPnt1bml0LnJlYWRpbmVzc1N0YXR1c308L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIGZvbnQtbW9ubyBiZy1zbGF0ZS05MDAvNjAgcHgtMiBweS0xIHJvdW5kZWRcIj5cbiAgICAgICAgICAgIDI0LzcgU+G6tW4gc8OgbmdcbiAgICAgICAgICA8L3NwYW4+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHsvKiBTcGF0aWFsIENvb3JkaW5hdGVzIEJveCAoVk4tMjAwMCAmIFdHUy04NCkgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZjE3MmFdIHAtMyByb3VuZGVkLWxnIGJvcmRlciBib3JkZXItc2xhdGUtNzAwLzcwIHNwYWNlLXktMiBmb250LW1vbm9cIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIHBiLTEuNSB0ZXh0LVsxMXB4XSB0ZXh0LWVtZXJhbGQtNDAwIGZvbnQtc2FucyBmb250LWJvbGRcIj5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGdhcC0xLjVcIj5cbiAgICAgICAgICAgICAgPENvbXBhc3MgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjUgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuPkThu64gTEnhu4ZVIFThu4xBIMSQ4buYIEdJUzwvc3Bhbj5cbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwXCI+RVBTRzozMjY0OCAmIDQzMjY8L3NwYW4+XG4gICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTIgZ2FwLTIgdGV4dC1bMTFweF1cIj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwIHRleHQtWzEwcHhdIGJsb2NrIGZvbnQtc2Fuc1wiPlThu41hIMSR4buZIFggKEVhc3RpbmcpOjwvc3Bhbj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1lbWVyYWxkLTMwMCBmb250LWJvbGRcIj57dW5pdC54X3ZuMjAwMC50b0xvY2FsZVN0cmluZygpfSBtPC9zcGFuPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCB0ZXh0LVsxMHB4XSBibG9jayBmb250LXNhbnNcIj5U4buNYSDEkeG7mSBZIChOb3J0aGluZyk6PC9zcGFuPlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LWVtZXJhbGQtMzAwIGZvbnQtYm9sZFwiPnt1bml0Lnlfdm4yMDAwLnRvTG9jYWxlU3RyaW5nKCl9IG08L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwIHRleHQtWzEwcHhdIGJsb2NrIGZvbnQtc2Fuc1wiPlbEqSDEkeG7mSAoV0dTODQpOjwvc3Bhbj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1za3ktMzAwXCI+e3VuaXQubGF0aXR1ZGUudG9GaXhlZCg2KX3CsE48L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwIHRleHQtWzEwcHhdIGJsb2NrIGZvbnQtc2Fuc1wiPktpbmggxJHhu5kgKFdHUzg0KTo8L3NwYW4+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2t5LTMwMFwiPnt1bml0LmxvbmdpdHVkZS50b0ZpeGVkKDYpfcKwRTwvc3Bhbj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogQ29tbWFuZGVyICYgQ29udGFjdCAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTIgYmctWyMwZjE3MmFdIHAtMyByb3VuZGVkLWxnIGJvcmRlciBib3JkZXItc2xhdGUtNzAwLzcwXCI+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLXN0YXJ0IGdhcC0yXCI+XG4gICAgICAgICAgICA8QXdhcmQgY2xhc3NOYW1lPVwidy00IGgtNCB0ZXh0LWFtYmVyLTQwMCBmbGV4LXNocmluay0wIG10LTAuNVwiIC8+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCBibG9ja1wiPkNo4buJIGh1eSB0csaw4bufbmcgLyBHacOhbSDEkeG7kWM6PC9zcGFuPlxuICAgICAgICAgICAgICA8c3Ryb25nIGNsYXNzTmFtZT1cInRleHQtd2hpdGUgdGV4dC14c1wiPnt1bml0LmNvbW1hbmRlciB8fCAnxJBhbmcgY+G6rXAgbmjhuq10J308L3N0cm9uZz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAge3VuaXQucG9saXRpY2FsQ29tbWlzc2FyICYmIChcbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1zdGFydCBnYXAtMiBib3JkZXItdCBib3JkZXItc2xhdGUtODAwIHB0LTJcIj5cbiAgICAgICAgICAgICAgPFNoaWVsZCBjbGFzc05hbWU9XCJ3LTQgaC00IHRleHQtcm9zZS00MDAgZmxleC1zaHJpbmstMCBtdC0wLjVcIiAvPlxuICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIGJsb2NrXCI+Q2jDrW5oIOG7p3kgLyBDaMOtbmggdHLhu4sgdmnDqm46PC9zcGFuPlxuICAgICAgICAgICAgICAgIDxzdHJvbmcgY2xhc3NOYW1lPVwidGV4dC13aGl0ZSB0ZXh0LXhzXCI+e3VuaXQucG9saXRpY2FsQ29tbWlzc2FyfTwvc3Ryb25nPlxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICl9XG5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGdhcC0yIGJvcmRlci10IGJvcmRlci1zbGF0ZS04MDAgcHQtMlwiPlxuICAgICAgICAgICAgPFBob25lIGNsYXNzTmFtZT1cInctNCBoLTQgdGV4dC1lbWVyYWxkLTQwMCBmbGV4LXNocmluay0wXCIgLz5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIGJsb2NrXCI+VHLhu7FjIGJhbiB0w6FjIGNoaeG6v246PC9zcGFuPlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC1lbWVyYWxkLTMwMCBmb250LWJvbGRcIj57dW5pdC5waG9uZX08L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1zdGFydCBnYXAtMiBib3JkZXItdCBib3JkZXItc2xhdGUtODAwIHB0LTJcIj5cbiAgICAgICAgICAgIDxNYXBQaW4gY2xhc3NOYW1lPVwidy00IGgtNCB0ZXh0LXNreS00MDAgZmxleC1zaHJpbmstMCBtdC0wLjVcIiAvPlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgYmxvY2tcIj7EkOG7i2EgY2jhu4kgxJHDs25nIHF1w6JuOjwvc3Bhbj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS0yMDBcIj57dW5pdC5hZGRyZXNzfTwvc3Bhbj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogTWlzc2lvbnMgKi99XG4gICAgICAgIHt1bml0Lm1haW5NaXNzaW9ucyAmJiB1bml0Lm1haW5NaXNzaW9ucy5sZW5ndGggPiAwICYmIChcbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInNwYWNlLXktMS41XCI+XG4gICAgICAgICAgICA8aDQgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gZm9udC1ib2xkIHRleHQtc2xhdGUtMzAwIHVwcGVyY2FzZSB0cmFja2luZy13aWRlciBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICAgIDxTaGllbGQgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjUgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuPk5oaeG7h20gduG7pSB0w6FjIGNoaeG6v24gY2jhu6cgeeG6v3U8L3NwYW4+XG4gICAgICAgICAgICA8L2g0PlxuICAgICAgICAgICAgPHVsIGNsYXNzTmFtZT1cInNwYWNlLXktMSBwbC00IGxpc3QtZGlzYyB0ZXh0LXNsYXRlLTMwMCB0ZXh0LVsxMXB4XVwiPlxuICAgICAgICAgICAgICB7dW5pdC5tYWluTWlzc2lvbnMubWFwKChtLCBpZHgpID0+IChcbiAgICAgICAgICAgICAgICA8bGkga2V5PXtpZHh9PnttfTwvbGk+XG4gICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgPC91bD5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgKX1cblxuICAgICAgICB7LyogV2VhcG9ucyAmIEVxdWlwbWVudCBIaWdobGlnaHRzICovfVxuICAgICAgICB7dW5pdC5lcXVpcG1lbnRIaWdobGlnaHRzICYmIHVuaXQuZXF1aXBtZW50SGlnaGxpZ2h0cy5sZW5ndGggPiAwICYmIChcbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInNwYWNlLXktMS41XCI+XG4gICAgICAgICAgICA8aDQgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gZm9udC1ib2xkIHRleHQtc2xhdGUtMzAwIHVwcGVyY2FzZSB0cmFja2luZy13aWRlciBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICAgIDxMYXllcnMgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjUgdGV4dC1hbWJlci00MDBcIiAvPlxuICAgICAgICAgICAgICA8c3Bhbj5Wxakga2jDrSwga2jDrSB0w6BpICYgdHJhbmcgYuG7iyBr4bu5IHRodeG6rXQ8L3NwYW4+XG4gICAgICAgICAgICA8L2g0PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGZsZXgtd3JhcCBnYXAtMS41IHB0LTAuNVwiPlxuICAgICAgICAgICAgICB7dW5pdC5lcXVpcG1lbnRIaWdobGlnaHRzLm1hcCgoZXEsIGlkeCkgPT4gKFxuICAgICAgICAgICAgICAgIDxzcGFuXG4gICAgICAgICAgICAgICAgICBrZXk9e2lkeH1cbiAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInB4LTIgcHktMC41IGJnLXNsYXRlLTgwMCB0ZXh0LXNsYXRlLTMwMCByb3VuZGVkIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHRleHQtWzEwcHhdXCJcbiAgICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgICB7ZXF9XG4gICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICApfVxuXG4gICAgICAgIHsvKiBOb3RlcyAqL31cbiAgICAgICAge3VuaXQubm90ZXMgJiYgKFxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC0yLjUgcm91bmRlZCBiZy1zbGF0ZS05MDAvNjAgYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgdGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgaXRhbGljXCI+XG4gICAgICAgICAgICBcInt1bml0Lm5vdGVzfVwiXG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICl9XG4gICAgICA8L2Rpdj5cblxuICAgICAgey8qIERyYXdlciBBY3Rpb24gRm9vdGVyICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJwLTMgYmctc2xhdGUtOTAwIGJvcmRlci10IGJvcmRlci1zbGF0ZS04MDAgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGdhcC0yXCI+XG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBvbkNlbnRlck1hcCh1bml0KX1cbiAgICAgICAgICBjbGFzc05hbWU9XCJmbGV4LTEgYmctc2xhdGUtODAwIGhvdmVyOmJnLXNsYXRlLTcwMCB0ZXh0LXdoaXRlIHRleHQteHMgZm9udC1zZW1pYm9sZCBweS0yIHB4LTMgcm91bmRlZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciBnYXAtMS41IHRyYW5zaXRpb25cIlxuICAgICAgICA+XG4gICAgICAgICAgPENvbXBhc3MgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjUgdGV4dC1za3ktNDAwXCIgLz5cbiAgICAgICAgICA8c3Bhbj7EkOG7i25oIHbhu4s8L3NwYW4+XG4gICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgb25DbGljaz17KCkgPT4gb25UcmlnZ2VyQnVmZmVyKHVuaXQpfVxuICAgICAgICAgIGNsYXNzTmFtZT1cImZsZXgtMSBiZy1hbWJlci03MDAvODAgaG92ZXI6YmctYW1iZXItNjAwIHRleHQtd2hpdGUgdGV4dC14cyBmb250LXNlbWlib2xkIHB5LTIgcHgtMyByb3VuZGVkIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIGdhcC0xLjUgdHJhbnNpdGlvblwiXG4gICAgICAgID5cbiAgICAgICAgICA8Q2lyY2xlRG90IGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICA8c3Bhbj5Ww7luZyDEkeG7h20gKDEwa20pPC9zcGFuPlxuICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9eygpID0+IG9uRWRpdCh1bml0KX1cbiAgICAgICAgICBjbGFzc05hbWU9XCJiZy1lbWVyYWxkLTYwMCBob3ZlcjpiZy1lbWVyYWxkLTUwMCB0ZXh0LXdoaXRlIHRleHQteHMgZm9udC1zZW1pYm9sZCBwLTIgcm91bmRlZCB0cmFuc2l0aW9uXCJcbiAgICAgICAgICB0aXRsZT1cIlPhu61hIHRow7RuZyB0aW4gxJHGoW4gduG7i1wiXG4gICAgICAgID5cbiAgICAgICAgICA8RWRpdCBjbGFzc05hbWU9XCJ3LTQgaC00XCIgLz5cbiAgICAgICAgPC9idXR0b24+XG4gICAgICA8L2Rpdj5cbiAgICA8L2Rpdj5cbiAgKTtcbn07XG4iXX0=