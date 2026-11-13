const React = __vite__cjsImport0_react; const useMemo = __vite__cjsImport0_react["useMemo"];const _jsxDEV = __vite__cjsImport3_react_jsxDevRuntime["jsxDEV"]; const _Fragment = __vite__cjsImport3_react_jsxDevRuntime["Fragment"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { auditMilitaryData } from "/src/utils/gisCalculations.ts";
import { CheckCircle2, AlertTriangle, XCircle, ShieldAlert, PlusCircle, Edit, Compass } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/DataCheckerView.tsx";
import __vite__cjsImport3_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const DataCheckerView = ({ units, onEditUnit, onSelectUnitAndGoToMap, onAddTestAnomaly }) => {
	const issues = useMemo(() => auditMilitaryData(units), [units]);
	const errorCount = issues.filter((i) => i.severity === "error").length;
	const warningCount = issues.filter((i) => i.severity === "warning").length;
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex-1 overflow-y-auto bg-[#080d19] text-white p-6 space-y-6",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex flex-wrap items-center justify-between gap-4 border-b border-slate-800 pb-4",
				children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h2", {
					className: "text-xl font-bold uppercase tracking-wider text-white flex items-center gap-2",
					children: [/* @__PURE__ */ _jsxDEV(ShieldAlert, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 39,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Kiểm tra CSDL & Topo Không gian (GIS Topology & Data Quality)" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 40,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 38,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("p", {
					className: "text-xs text-slate-400 mt-1",
					children: "Tự động rà soát sai số tọa độ nằm ngoài ranh giới Hà Nội, trùng lặp mã quân sự và thiếu trường thuộc tính"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 42,
					columnNumber: 11
				}, this)] }, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 37,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("button", {
					onClick: onAddTestAnomaly,
					title: "Tạo thử 1 bản ghi lỗi ngoài Hà Nội để kiểm nghiệm thuật toán",
					className: "bg-slate-800 hover:bg-slate-700 text-amber-300 text-xs font-semibold px-3.5 py-2 rounded-lg flex items-center gap-2 border border-slate-700 transition",
					children: [/* @__PURE__ */ _jsxDEV(PlusCircle, { className: "w-4 h-4 text-amber-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 53,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Thêm bản ghi thử nghiệm lỗi" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 54,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 48,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 36,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "grid grid-cols-1 md:grid-cols-3 gap-4",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "Trạng thái Topo GIS"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 63,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: `text-xl font-bold mt-1 ${issues.length === 0 ? "text-emerald-400" : "text-amber-400"}`,
								children: issues.length === 0 ? "Toàn vẹn 100%" : `Phát hiện ${issues.length} vấn đề`
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 64,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-slate-400 mt-0.5",
								children: "Chuẩn EPSG:32648 & Polygon Hà Nội"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 71,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 62,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: `w-12 h-12 rounded-lg flex items-center justify-center ${issues.length === 0 ? "bg-emerald-950/60 border border-emerald-700/50 text-emerald-400" : "bg-amber-950/60 border border-amber-700/50 text-amber-400"}`,
							children: issues.length === 0 ? /* @__PURE__ */ _jsxDEV(CheckCircle2, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 83,
								columnNumber: 15
							}, this) : /* @__PURE__ */ _jsxDEV(AlertTriangle, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 85,
								columnNumber: 15
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 75,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 61,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "Lỗi nghiêm trọng (Errors)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 93,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-2xl font-bold text-rose-400 mt-1",
								children: errorCount
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 94,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-slate-400 mt-0.5",
								children: "Ngoài ranh giới / Trùng khóa chính"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 95,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 92,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "w-12 h-12 rounded-lg bg-rose-950/60 border border-rose-700/50 flex items-center justify-center text-rose-400",
							children: /* @__PURE__ */ _jsxDEV(XCircle, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 98,
								columnNumber: 13
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 97,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 91,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "Cảnh báo (Warnings)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 105,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-2xl font-bold text-amber-400 mt-1",
								children: warningCount
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 106,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-slate-400 mt-0.5",
								children: "Trùng tọa độ / Thiếu thông tin liên hệ"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 107,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 104,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "w-12 h-12 rounded-lg bg-amber-950/60 border border-amber-700/50 flex items-center justify-center text-amber-400",
							children: /* @__PURE__ */ _jsxDEV(AlertTriangle, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 110,
								columnNumber: 13
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 109,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 103,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 59,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 shadow-lg space-y-4",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "flex items-center justify-between border-b border-slate-800 pb-3",
					children: [/* @__PURE__ */ _jsxDEV("h3", {
						className: "text-sm font-bold text-white uppercase tracking-wider flex items-center gap-2",
						children: /* @__PURE__ */ _jsxDEV("span", { children: [
							"Danh mục Cảnh báo & Đề xuất Khắc phục (",
							issues.length,
							")"
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 119,
							columnNumber: 13
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 118,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("span", {
						className: "text-xs text-slate-400",
						children: [
							"Kiểm tra tự động trên ",
							units.length,
							" đối tượng hình học"
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 121,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 117,
					columnNumber: 9
				}, this), issues.length === 0 ? /* @__PURE__ */ _jsxDEV("div", {
					className: "p-8 text-center space-y-3",
					children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "w-14 h-14 rounded-full bg-emerald-950/60 border border-emerald-500/50 text-emerald-400 flex items-center justify-center mx-auto",
							children: /* @__PURE__ */ _jsxDEV(CheckCircle2, { className: "w-7 h-7" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 129,
								columnNumber: 15
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 128,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("h4", {
							className: "text-base font-bold text-white",
							children: "Cơ sở Dữ liệu Đạt Chuẩn Hình học & Thuộc tính GIS!"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 131,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("p", {
							className: "text-xs text-slate-400 max-w-lg mx-auto",
							children: "Tất cả 100% đơn vị quân đội đều có tọa độ hợp lệ nằm hoàn toàn bên trong ranh giới hành chính thành phố Hà Nội, mã định danh duy nhất và đầy đủ thông tin chỉ huy trực ban tác chiến."
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 134,
							columnNumber: 13
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 127,
					columnNumber: 11
				}, this) : /* @__PURE__ */ _jsxDEV("div", {
					className: "space-y-3",
					children: issues.map((issue) => {
						const matchedUnit = units.find((u) => u.id === issue.unitId);
						return /* @__PURE__ */ _jsxDEV("div", {
							className: `p-4 rounded-xl border flex flex-col md:flex-row md:items-center justify-between gap-4 ${issue.severity === "error" ? "bg-rose-950/20 border-rose-800/60" : "bg-amber-950/20 border-amber-800/60"}`,
							children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "space-y-1",
								children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center gap-2",
										children: [
											/* @__PURE__ */ _jsxDEV("span", {
												className: `text-[10px] uppercase font-bold px-2 py-0.5 rounded font-mono ${issue.severity === "error" ? "bg-rose-900 text-rose-200" : "bg-amber-900 text-amber-200"}`,
												children: issue.severity === "error" ? "LỖI HÌNH HỌC" : "CẢNH BÁO"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 156,
												columnNumber: 23
											}, this),
											/* @__PURE__ */ _jsxDEV("span", {
												className: "font-mono text-xs text-slate-300 font-bold",
												children: [
													"[",
													issue.unitCode,
													"]"
												]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 165,
												columnNumber: 23
											}, this),
											/* @__PURE__ */ _jsxDEV("strong", {
												className: "text-white text-xs",
												children: issue.unitName
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 168,
												columnNumber: 23
											}, this)
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 155,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-xs text-slate-200 font-medium",
										children: issue.description
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 171,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-[11px] text-emerald-400 flex items-center gap-1 mt-1",
										children: [/* @__PURE__ */ _jsxDEV("span", { children: "💡 Gợi ý xử lý:" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 174,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: issue.suggestion }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 175,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 173,
										columnNumber: 21
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 154,
								columnNumber: 19
							}, this), /* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-center gap-2 flex-shrink-0",
								children: matchedUnit && /* @__PURE__ */ _jsxDEV(_Fragment, { children: [/* @__PURE__ */ _jsxDEV("button", {
									onClick: () => onSelectUnitAndGoToMap(matchedUnit),
									className: "bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs px-3 py-1.5 rounded flex items-center gap-1 transition",
									children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-3.5 h-3.5 text-sky-400" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 186,
										columnNumber: 27
									}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Xem vị trí" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 187,
										columnNumber: 27
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 182,
									columnNumber: 25
								}, this), /* @__PURE__ */ _jsxDEV("button", {
									onClick: () => onEditUnit(matchedUnit),
									className: "bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold px-3 py-1.5 rounded flex items-center gap-1 transition",
									children: [/* @__PURE__ */ _jsxDEV(Edit, { className: "w-3.5 h-3.5" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 193,
										columnNumber: 27
									}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Sửa thông tin" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 194,
										columnNumber: 27
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 189,
									columnNumber: 25
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 181,
									columnNumber: 23
								}, this)
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 179,
								columnNumber: 19
							}, this)]
						}, issue.id, true, {
							fileName: _jsxFileName,
							lineNumber: 146,
							columnNumber: 17
						}, this);
					})
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 141,
					columnNumber: 11
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 116,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 space-y-2 text-xs text-slate-400",
				children: [/* @__PURE__ */ _jsxDEV("h4", {
					className: "text-xs font-bold text-white uppercase flex items-center gap-2",
					children: /* @__PURE__ */ _jsxDEV("span", { children: "Quy tắc Kiểm thử Dữ liệu Không gian Quân sự:" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 209,
						columnNumber: 11
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 208,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("ul", {
					className: "list-disc pl-5 space-y-1",
					children: [
						/* @__PURE__ */ _jsxDEV("li", { children: [/* @__PURE__ */ _jsxDEV("strong", { children: "Ranh giới hành chính:" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 213,
							columnNumber: 13
						}, this), " Tọa độ WGS84 phải thỏa mãn Ray-Casting nằm bên trong đa giác đường bao ngoại vi của TP Hà Nội (Vĩ độ: 20.55° - 21.40°N, Kinh độ: 105.25° - 106.10°E)."] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 212,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("li", { children: [/* @__PURE__ */ _jsxDEV("strong", { children: "Hệ quy chiếu:" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 218,
							columnNumber: 13
						}, this), " Chuyển đổi hai chiều chính xác giữa WGS84 (EPSG:4326) và VN-2000 Múi chiếu 3 độ / UTM 48N (EPSG:32648)."] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 217,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("li", { children: [/* @__PURE__ */ _jsxDEV("strong", { children: "Toàn vẹn thực thể (Entity Integrity):" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 222,
							columnNumber: 13
						}, this), " Mã đơn vị là duy nhất, không trùng lặp trong CSDL PostGIS."] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 221,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 211,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 207,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 34,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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V0FBaUI7Ozs7a0JBQzNCOzs7Ozs7UUFDRjs7Ozs7aUJBRUwsd0JBQUMsT0FBRDtRQUFLLFdBQVU7a0JBQ1osZUFDQyxnREFDRSx3QkFBQyxVQUFEO1NBQ0UsZUFBZSx1QkFBdUIsV0FBVztTQUNqRCxXQUFVO21CQUZaLENBSUUsd0JBQUMsU0FBRCxFQUFTLFdBQVUsMkJBQTRCOzs7O21CQUMvQyx3QkFBQyxRQUFELFlBQU0sYUFBZ0I7Ozs7aUJBQ2hCOzs7OztrQkFDUix3QkFBQyxVQUFEO1NBQ0UsZUFBZSxXQUFXLFdBQVc7U0FDckMsV0FBVTttQkFGWixDQUlFLHdCQUFDLE1BQUQsRUFBTSxXQUFVLGNBQWU7Ozs7bUJBQy9CLHdCQUFDLFFBQUQsWUFBTSxnQkFBbUI7Ozs7aUJBQ25COzs7OztnQkFDUjs7Ozs7T0FFRDs7OztlQUNGO1NBcERFLE1BQU07Ozs7YUFvRFI7S0FFVCxDQUFDO0lBQ0U7Ozs7WUFFSjs7Ozs7O0dBR0wsd0JBQUMsT0FBRDtJQUFLLFdBQVU7Y0FBZixDQUNFLHdCQUFDLE1BQUQ7S0FBSSxXQUFVO2VBQ1osd0JBQUMsUUFBRCxZQUFNLCtDQUFrRDs7Ozs7SUFDdEQ7Ozs7Y0FDSix3QkFBQyxNQUFEO0tBQUksV0FBVTtlQUFkO01BQ0Usd0JBQUMsTUFBRCxhQUNFLHdCQUFDLFVBQUQsWUFBUSx3QkFBNkI7Ozs7Z0JBQUMsd0pBR3BDOzs7OztNQUNKLHdCQUFDLE1BQUQsYUFDRSx3QkFBQyxVQUFELFlBQVEsZ0JBQXFCOzs7O2dCQUFDLDBHQUU1Qjs7Ozs7TUFDSix3QkFBQyxNQUFELGFBQ0Usd0JBQUMsVUFBRCxZQUFRLHdDQUE2Qzs7OztnQkFBQyw2REFFcEQ7Ozs7O0tBQ0Y7Ozs7O1lBQ0Q7Ozs7OztFQUNGOzs7Ozs7QUFFVCIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJEYXRhQ2hlY2tlclZpZXcudHN4Il0sInZlcnNpb24iOjMsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBSZWFjdCwgeyB1c2VNZW1vIH0gZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0LCBUb3BvbG9neUlzc3VlIH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IGF1ZGl0TWlsaXRhcnlEYXRhIH0gZnJvbSAnLi4vdXRpbHMvZ2lzQ2FsY3VsYXRpb25zJztcbmltcG9ydCB7XG4gIENoZWNrQ2lyY2xlMixcbiAgQWxlcnRUcmlhbmdsZSxcbiAgWENpcmNsZSxcbiAgU2hpZWxkQWxlcnQsXG4gIFJvdGF0ZUNjdyxcbiAgUGx1c0NpcmNsZSxcbiAgRWRpdCxcbiAgQ29tcGFzcyxcbn0gZnJvbSAnbHVjaWRlLXJlYWN0JztcblxuaW50ZXJmYWNlIERhdGFDaGVja2VyVmlld1Byb3BzIHtcbiAgdW5pdHM6IE1pbGl0YXJ5VW5pdFtdO1xuICBvbkVkaXRVbml0OiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xuICBvblNlbGVjdFVuaXRBbmRHb1RvTWFwOiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xuICBvbkFkZFRlc3RBbm9tYWx5OiAoKSA9PiB2b2lkO1xufVxuXG5leHBvcnQgY29uc3QgRGF0YUNoZWNrZXJWaWV3OiBSZWFjdC5GQzxEYXRhQ2hlY2tlclZpZXdQcm9wcz4gPSAoe1xuICB1bml0cyxcbiAgb25FZGl0VW5pdCxcbiAgb25TZWxlY3RVbml0QW5kR29Ub01hcCxcbiAgb25BZGRUZXN0QW5vbWFseSxcbn0pID0+IHtcbiAgY29uc3QgaXNzdWVzID0gdXNlTWVtbygoKSA9PiBhdWRpdE1pbGl0YXJ5RGF0YSh1bml0cyksIFt1bml0c10pO1xuXG4gIGNvbnN0IGVycm9yQ291bnQgPSBpc3N1ZXMuZmlsdGVyKChpKSA9PiBpLnNldmVyaXR5ID09PSAnZXJyb3InKS5sZW5ndGg7XG4gIGNvbnN0IHdhcm5pbmdDb3VudCA9IGlzc3Vlcy5maWx0ZXIoKGkpID0+IGkuc2V2ZXJpdHkgPT09ICd3YXJuaW5nJykubGVuZ3RoO1xuXG4gIHJldHVybiAoXG4gICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4LTEgb3ZlcmZsb3cteS1hdXRvIGJnLVsjMDgwZDE5XSB0ZXh0LXdoaXRlIHAtNiBzcGFjZS15LTZcIj5cbiAgICAgIHsvKiBUaXRsZSAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBmbGV4LXdyYXAgaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBnYXAtNCBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIHBiLTRcIj5cbiAgICAgICAgPGRpdj5cbiAgICAgICAgICA8aDIgY2xhc3NOYW1lPVwidGV4dC14bCBmb250LWJvbGQgdXBwZXJjYXNlIHRyYWNraW5nLXdpZGVyIHRleHQtd2hpdGUgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgIDxTaGllbGRBbGVydCBjbGFzc05hbWU9XCJ3LTUgaC01IHRleHQtZW1lcmFsZC00MDBcIiAvPlxuICAgICAgICAgICAgPHNwYW4+S2nhu4NtIHRyYSBDU0RMICYgVG9wbyBLaMO0bmcgZ2lhbiAoR0lTIFRvcG9sb2d5ICYgRGF0YSBRdWFsaXR5KTwvc3Bhbj5cbiAgICAgICAgICA8L2gyPlxuICAgICAgICAgIDxwIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlxuICAgICAgICAgICAgVOG7sSDEkeG7mW5nIHLDoCBzb8OhdCBzYWkgc+G7kSB04buNYSDEkeG7mSBu4bqxbSBuZ2/DoGkgcmFuaCBnaeG7m2kgSMOgIE7hu5lpLCB0csO5bmcgbOG6t3AgbcOjIHF1w6JuIHPhu7EgdsOgIHRoaeG6v3VcbiAgICAgICAgICAgIHRyxrDhu51uZyB0aHXhu5ljIHTDrW5oXG4gICAgICAgICAgPC9wPlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgb25DbGljaz17b25BZGRUZXN0QW5vbWFseX1cbiAgICAgICAgICB0aXRsZT1cIlThuqFvIHRo4butIDEgYuG6o24gZ2hpIGzhu5dpIG5nb8OgaSBIw6AgTuG7mWkgxJHhu4Mga2nhu4NtIG5naGnhu4dtIHRodeG6rXQgdG/DoW5cIlxuICAgICAgICAgIGNsYXNzTmFtZT1cImJnLXNsYXRlLTgwMCBob3ZlcjpiZy1zbGF0ZS03MDAgdGV4dC1hbWJlci0zMDAgdGV4dC14cyBmb250LXNlbWlib2xkIHB4LTMuNSBweS0yIHJvdW5kZWQtbGcgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTIgYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgdHJhbnNpdGlvblwiXG4gICAgICAgID5cbiAgICAgICAgICA8UGx1c0NpcmNsZSBjbGFzc05hbWU9XCJ3LTQgaC00IHRleHQtYW1iZXItNDAwXCIgLz5cbiAgICAgICAgICA8c3Bhbj5UaMOqbSBi4bqjbiBnaGkgdGjhu60gbmdoaeG7h20gbOG7l2k8L3NwYW4+XG4gICAgICAgIDwvYnV0dG9uPlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBTdW1tYXJ5IENhcmRzICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIG1kOmdyaWQtY29scy0zIGdhcC00XCI+XG4gICAgICAgIHsvKiBTdGF0dXMgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC00IHNoYWRvdy1tZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW5cIj5cbiAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGZvbnQtbWVkaXVtXCI+VHLhuqFuZyB0aMOhaSBUb3BvIEdJUzwvZGl2PlxuICAgICAgICAgICAgPGRpdlxuICAgICAgICAgICAgICBjbGFzc05hbWU9e2B0ZXh0LXhsIGZvbnQtYm9sZCBtdC0xICR7XG4gICAgICAgICAgICAgICAgaXNzdWVzLmxlbmd0aCA9PT0gMCA/ICd0ZXh0LWVtZXJhbGQtNDAwJyA6ICd0ZXh0LWFtYmVyLTQwMCdcbiAgICAgICAgICAgICAgfWB9XG4gICAgICAgICAgICA+XG4gICAgICAgICAgICAgIHtpc3N1ZXMubGVuZ3RoID09PSAwID8gJ1Rvw6BuIHbhurluIDEwMCUnIDogYFBow6F0IGhp4buHbiAke2lzc3Vlcy5sZW5ndGh9IHbhuqVuIMSR4buBYH1cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSB0ZXh0LXNsYXRlLTQwMCBtdC0wLjVcIj5cbiAgICAgICAgICAgICAgQ2h14bqpbiBFUFNHOjMyNjQ4ICYgUG9seWdvbiBIw6AgTuG7mWlcbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXZcbiAgICAgICAgICAgIGNsYXNzTmFtZT17YHctMTIgaC0xMiByb3VuZGVkLWxnIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyICR7XG4gICAgICAgICAgICAgIGlzc3Vlcy5sZW5ndGggPT09IDBcbiAgICAgICAgICAgICAgICA/ICdiZy1lbWVyYWxkLTk1MC82MCBib3JkZXIgYm9yZGVyLWVtZXJhbGQtNzAwLzUwIHRleHQtZW1lcmFsZC00MDAnXG4gICAgICAgICAgICAgICAgOiAnYmctYW1iZXItOTUwLzYwIGJvcmRlciBib3JkZXItYW1iZXItNzAwLzUwIHRleHQtYW1iZXItNDAwJ1xuICAgICAgICAgICAgfWB9XG4gICAgICAgICAgPlxuICAgICAgICAgICAge2lzc3Vlcy5sZW5ndGggPT09IDAgPyAoXG4gICAgICAgICAgICAgIDxDaGVja0NpcmNsZTIgY2xhc3NOYW1lPVwidy02IGgtNlwiIC8+XG4gICAgICAgICAgICApIDogKFxuICAgICAgICAgICAgICA8QWxlcnRUcmlhbmdsZSBjbGFzc05hbWU9XCJ3LTYgaC02XCIgLz5cbiAgICAgICAgICAgICl9XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHsvKiBFcnJvcnMgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC00IHNoYWRvdy1tZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW5cIj5cbiAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGZvbnQtbWVkaXVtXCI+TOG7l2kgbmdoacOqbSB0cuG7jW5nIChFcnJvcnMpPC9kaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtMnhsIGZvbnQtYm9sZCB0ZXh0LXJvc2UtNDAwIG10LTFcIj57ZXJyb3JDb3VudH08L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMC41XCI+Tmdvw6BpIHJhbmggZ2nhu5tpIC8gVHLDuW5nIGtow7NhIGNow61uaDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidy0xMiBoLTEyIHJvdW5kZWQtbGcgYmctcm9zZS05NTAvNjAgYm9yZGVyIGJvcmRlci1yb3NlLTcwMC81MCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0ZXh0LXJvc2UtNDAwXCI+XG4gICAgICAgICAgICA8WENpcmNsZSBjbGFzc05hbWU9XCJ3LTYgaC02XCIgLz5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgey8qIFdhcm5pbmdzICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIHAtNCBzaGFkb3ctbWQgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuXCI+XG4gICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LXNsYXRlLTQwMCBmb250LW1lZGl1bVwiPkPhuqNuaCBiw6FvIChXYXJuaW5ncyk8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC0yeGwgZm9udC1ib2xkIHRleHQtYW1iZXItNDAwIG10LTFcIj57d2FybmluZ0NvdW50fTwvZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSB0ZXh0LXNsYXRlLTQwMCBtdC0wLjVcIj5UcsO5bmcgdOG7jWEgxJHhu5kgLyBUaGnhur91IHRow7RuZyB0aW4gbGnDqm4gaOG7hzwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidy0xMiBoLTEyIHJvdW5kZWQtbGcgYmctYW1iZXItOTUwLzYwIGJvcmRlciBib3JkZXItYW1iZXItNzAwLzUwIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIHRleHQtYW1iZXItNDAwXCI+XG4gICAgICAgICAgICA8QWxlcnRUcmlhbmdsZSBjbGFzc05hbWU9XCJ3LTYgaC02XCIgLz5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG4gICAgICA8L2Rpdj5cblxuICAgICAgey8qIElzc3VlcyBUYWJsZSBvciBDbGVhbiBTdGF0ZSAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC01IHNoYWRvdy1sZyBzcGFjZS15LTRcIj5cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi0zXCI+XG4gICAgICAgICAgPGgzIGNsYXNzTmFtZT1cInRleHQtc20gZm9udC1ib2xkIHRleHQtd2hpdGUgdXBwZXJjYXNlIHRyYWNraW5nLXdpZGVyIGZsZXggaXRlbXMtY2VudGVyIGdhcC0yXCI+XG4gICAgICAgICAgICA8c3Bhbj5EYW5oIG3hu6VjIEPhuqNuaCBiw6FvICYgxJDhu4EgeHXhuqV0IEto4bqvYyBwaOG7pWMgKHtpc3N1ZXMubGVuZ3RofSk8L3NwYW4+XG4gICAgICAgICAgPC9oMz5cbiAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwXCI+XG4gICAgICAgICAgICBLaeG7g20gdHJhIHThu7EgxJHhu5luZyB0csOqbiB7dW5pdHMubGVuZ3RofSDEkeG7kWkgdMaw4bujbmcgaMOsbmggaOG7jWNcbiAgICAgICAgICA8L3NwYW4+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHtpc3N1ZXMubGVuZ3RoID09PSAwID8gKFxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC04IHRleHQtY2VudGVyIHNwYWNlLXktM1wiPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ3LTE0IGgtMTQgcm91bmRlZC1mdWxsIGJnLWVtZXJhbGQtOTUwLzYwIGJvcmRlciBib3JkZXItZW1lcmFsZC01MDAvNTAgdGV4dC1lbWVyYWxkLTQwMCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciBteC1hdXRvXCI+XG4gICAgICAgICAgICAgIDxDaGVja0NpcmNsZTIgY2xhc3NOYW1lPVwidy03IGgtN1wiIC8+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxoNCBjbGFzc05hbWU9XCJ0ZXh0LWJhc2UgZm9udC1ib2xkIHRleHQtd2hpdGVcIj5cbiAgICAgICAgICAgICAgQ8ahIHPhu58gROG7ryBsaeG7h3UgxJDhuqF0IENodeG6qW4gSMOsbmggaOG7jWMgJiBUaHXhu5ljIHTDrW5oIEdJUyFcbiAgICAgICAgICAgIDwvaDQ+XG4gICAgICAgICAgICA8cCBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIG1heC13LWxnIG14LWF1dG9cIj5cbiAgICAgICAgICAgICAgVOG6pXQgY+G6oyAxMDAlIMSRxqFuIHbhu4sgcXXDom4gxJHhu5lpIMSR4buBdSBjw7MgdOG7jWEgxJHhu5kgaOG7o3AgbOG7hyBu4bqxbSBob8OgbiB0b8OgbiBiw6puIHRyb25nIHJhbmggZ2nhu5tpIGjDoG5oXG4gICAgICAgICAgICAgIGNow61uaCB0aMOgbmggcGjhu5EgSMOgIE7hu5lpLCBtw6MgxJHhu4tuaCBkYW5oIGR1eSBuaOG6pXQgdsOgIMSR4bqneSDEkeG7pyB0aMO0bmcgdGluIGNo4buJIGh1eSB0cuG7sWMgYmFuIHTDoWNcbiAgICAgICAgICAgICAgY2hp4bq/bi5cbiAgICAgICAgICAgIDwvcD5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgKSA6IChcbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInNwYWNlLXktM1wiPlxuICAgICAgICAgICAge2lzc3Vlcy5tYXAoKGlzc3VlKSA9PiB7XG4gICAgICAgICAgICAgIGNvbnN0IG1hdGNoZWRVbml0ID0gdW5pdHMuZmluZCgodSkgPT4gdS5pZCA9PT0gaXNzdWUudW5pdElkKTtcblxuICAgICAgICAgICAgICByZXR1cm4gKFxuICAgICAgICAgICAgICAgIDxkaXZcbiAgICAgICAgICAgICAgICAgIGtleT17aXNzdWUuaWR9XG4gICAgICAgICAgICAgICAgICBjbGFzc05hbWU9e2BwLTQgcm91bmRlZC14bCBib3JkZXIgZmxleCBmbGV4LWNvbCBtZDpmbGV4LXJvdyBtZDppdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGdhcC00ICR7XG4gICAgICAgICAgICAgICAgICAgIGlzc3VlLnNldmVyaXR5ID09PSAnZXJyb3InXG4gICAgICAgICAgICAgICAgICAgICAgPyAnYmctcm9zZS05NTAvMjAgYm9yZGVyLXJvc2UtODAwLzYwJ1xuICAgICAgICAgICAgICAgICAgICAgIDogJ2JnLWFtYmVyLTk1MC8yMCBib3JkZXItYW1iZXItODAwLzYwJ1xuICAgICAgICAgICAgICAgICAgfWB9XG4gICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTFcIj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuXG4gICAgICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9e2B0ZXh0LVsxMHB4XSB1cHBlcmNhc2UgZm9udC1ib2xkIHB4LTIgcHktMC41IHJvdW5kZWQgZm9udC1tb25vICR7XG4gICAgICAgICAgICAgICAgICAgICAgICAgIGlzc3VlLnNldmVyaXR5ID09PSAnZXJyb3InXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPyAnYmctcm9zZS05MDAgdGV4dC1yb3NlLTIwMCdcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA6ICdiZy1hbWJlci05MDAgdGV4dC1hbWJlci0yMDAnXG4gICAgICAgICAgICAgICAgICAgICAgICB9YH1cbiAgICAgICAgICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgICAgICAgICB7aXNzdWUuc2V2ZXJpdHkgPT09ICdlcnJvcicgPyAnTOG7lkkgSMOMTkggSOG7jEMnIDogJ0PhuqJOSCBCw4FPJ31cbiAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwiZm9udC1tb25vIHRleHQteHMgdGV4dC1zbGF0ZS0zMDAgZm9udC1ib2xkXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICBbe2lzc3VlLnVuaXRDb2RlfV1cbiAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPHN0cm9uZyBjbGFzc05hbWU9XCJ0ZXh0LXdoaXRlIHRleHQteHNcIj57aXNzdWUudW5pdE5hbWV9PC9zdHJvbmc+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LXNsYXRlLTIwMCBmb250LW1lZGl1bVwiPntpc3N1ZS5kZXNjcmlwdGlvbn08L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtWzExcHhdIHRleHQtZW1lcmFsZC00MDAgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEgbXQtMVwiPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuPvCfkqEgR+G7o2kgw70geOG7rSBsw706PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuPntpc3N1ZS5zdWdnZXN0aW9ufTwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMiBmbGV4LXNocmluay0wXCI+XG4gICAgICAgICAgICAgICAgICAgIHttYXRjaGVkVW5pdCAmJiAoXG4gICAgICAgICAgICAgICAgICAgICAgPD5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgICAgICAgICAgICAgb25DbGljaz17KCkgPT4gb25TZWxlY3RVbml0QW5kR29Ub01hcChtYXRjaGVkVW5pdCl9XG4gICAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLXNsYXRlLTgwMCBob3ZlcjpiZy1zbGF0ZS03MDAgdGV4dC1zbGF0ZS0yMDAgdGV4dC14cyBweC0zIHB5LTEuNSByb3VuZGVkIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xIHRyYW5zaXRpb25cIlxuICAgICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAgICA8Q29tcGFzcyBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LXNreS00MDBcIiAvPlxuICAgICAgICAgICAgICAgICAgICAgICAgICA8c3Bhbj5YZW0gduG7iyB0csOtPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IG9uRWRpdFVuaXQobWF0Y2hlZFVuaXQpfVxuICAgICAgICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJiZy1lbWVyYWxkLTYwMCBob3ZlcjpiZy1lbWVyYWxkLTUwMCB0ZXh0LXdoaXRlIHRleHQteHMgZm9udC1zZW1pYm9sZCBweC0zIHB5LTEuNSByb3VuZGVkIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xIHRyYW5zaXRpb25cIlxuICAgICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAgICA8RWRpdCBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNVwiIC8+XG4gICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuPlPhu61hIHRow7RuZyB0aW48L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICAgICAgICA8Lz5cbiAgICAgICAgICAgICAgICAgICAgKX1cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgfSl9XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICl9XG4gICAgICA8L2Rpdj5cblxuICAgICAgey8qIEdJUyBWZXJpZmljYXRpb24gUnVsZXMgR3VpZGUgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIHAtNSBzcGFjZS15LTIgdGV4dC14cyB0ZXh0LXNsYXRlLTQwMFwiPlxuICAgICAgICA8aDQgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdGV4dC13aGl0ZSB1cHBlcmNhc2UgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICA8c3Bhbj5RdXkgdOG6r2MgS2nhu4NtIHRo4butIEThu68gbGnhu4d1IEtow7RuZyBnaWFuIFF1w6JuIHPhu7E6PC9zcGFuPlxuICAgICAgICA8L2g0PlxuICAgICAgICA8dWwgY2xhc3NOYW1lPVwibGlzdC1kaXNjIHBsLTUgc3BhY2UteS0xXCI+XG4gICAgICAgICAgPGxpPlxuICAgICAgICAgICAgPHN0cm9uZz5SYW5oIGdp4bubaSBow6BuaCBjaMOtbmg6PC9zdHJvbmc+IFThu41hIMSR4buZIFdHUzg0IHBo4bqjaSB0aOG7j2EgbcOjbiBSYXktQ2FzdGluZyBu4bqxbSBiw6puXG4gICAgICAgICAgICB0cm9uZyDEkWEgZ2nDoWMgxJHGsOG7nW5nIGJhbyBuZ2/huqFpIHZpIGPhu6dhIFRQIEjDoCBO4buZaSAoVsSpIMSR4buZOiAyMC41NcKwIC0gMjEuNDDCsE4sIEtpbmggxJHhu5k6IDEwNS4yNcKwXG4gICAgICAgICAgICAtIDEwNi4xMMKwRSkuXG4gICAgICAgICAgPC9saT5cbiAgICAgICAgICA8bGk+XG4gICAgICAgICAgICA8c3Ryb25nPkjhu4cgcXV5IGNoaeG6v3U6PC9zdHJvbmc+IENodXnhu4NuIMSR4buVaSBoYWkgY2hp4buBdSBjaMOtbmggeMOhYyBnaeG7r2EgV0dTODQgKEVQU0c6NDMyNikgdsOgXG4gICAgICAgICAgICBWTi0yMDAwIE3DumkgY2hp4bq/dSAzIMSR4buZIC8gVVRNIDQ4TiAoRVBTRzozMjY0OCkuXG4gICAgICAgICAgPC9saT5cbiAgICAgICAgICA8bGk+XG4gICAgICAgICAgICA8c3Ryb25nPlRvw6BuIHbhurluIHRo4buxYyB0aOG7gyAoRW50aXR5IEludGVncml0eSk6PC9zdHJvbmc+IE3DoyDEkcahbiB24buLIGzDoCBkdXkgbmjhuqV0LCBraMO0bmcgdHLDuW5nXG4gICAgICAgICAgICBs4bq3cCB0cm9uZyBDU0RMIFBvc3RHSVMuXG4gICAgICAgICAgPC9saT5cbiAgICAgICAgPC91bD5cbiAgICAgIDwvZGl2PlxuICAgIDwvZGl2PlxuICApO1xufTtcbiJdfQ==