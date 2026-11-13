const React = __vite__cjsImport0_react;const _jsxDEV = __vite__cjsImport2_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { Shield, Activity, Layers, MapPin, Compass, Award, ArrowRight, Database, Building } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/DashboardView.tsx";
import __vite__cjsImport2_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const DashboardView = ({ units, onNavigateToMap, onFilterBranch, onSelectUnit }) => {
	// Compute analytics
	const totalUnits = units.length;
	const level1Count = units.filter((u) => u.readinessStatus.includes("Cấp 1")).length;
	const level2Count = units.filter((u) => u.readinessStatus.includes("Cấp 2")).length;
	const trainingCount = units.filter((u) => u.readinessStatus.includes("Huấn luyện")).length;
	// Branch statistics
	const branchCounts = units.reduce((acc, u) => {
		acc[u.branch] = (acc[u.branch] || 0) + 1;
		return acc;
	}, {});
	const sortedBranches = Object.entries(branchCounts).sort((a, b) => b[1] - a[1]);
	// District distribution
	const districtCounts = units.reduce((acc, u) => {
		acc[u.district] = (acc[u.district] || 0) + 1;
		return acc;
	}, {});
	const sortedDistricts = Object.entries(districtCounts).sort((a, b) => b[1] - a[1]);
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex-1 overflow-y-auto bg-[#080d19] text-white p-6 space-y-6",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "relative overflow-hidden rounded-xl bg-gradient-to-r from-emerald-950/70 via-slate-900 to-slate-900 border border-emerald-800/40 p-6 shadow-xl",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "relative z-10 max-w-3xl",
					children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "inline-flex items-center gap-2 px-3 py-1 rounded-full bg-emerald-900/50 border border-emerald-500/30 text-emerald-300 text-xs font-semibold mb-3",
							children: [/* @__PURE__ */ _jsxDEV("span", { className: "w-2 h-2 rounded-full bg-emerald-400 animate-pulse" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 58,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Hệ thống Cơ sở dữ liệu Không gian GIS Quân sự" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 59,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 57,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("h2", {
							className: "text-2xl font-extrabold tracking-tight text-white sm:text-3xl",
							children: "Tổng quan Cơ sở dữ liệu Đơn vị Quân đội trên Địa bàn Hà Nội"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 61,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("p", {
							className: "mt-2 text-sm text-slate-300 leading-relaxed",
							children: "Hệ thống cung cấp bức tranh toàn cảnh về phân bố không gian, trạng thái sẵn sàng chiến đấu, tọa độ chuẩn VN-2000 (EPSG:32648) và công cụ phân tích vùng đệm tác chiến của các đơn vị quân đội trên 30 quận, huyện, thị xã thuộc thành phố Hà Nội."
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 64,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "mt-5 flex flex-wrap gap-3",
							children: [/* @__PURE__ */ _jsxDEV("button", {
								onClick: onNavigateToMap,
								className: "bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-xs px-4 py-2.5 rounded-lg flex items-center gap-2 shadow-lg transition active:scale-95",
								children: [
									/* @__PURE__ */ _jsxDEV(Compass, { className: "w-4 h-4" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 74,
										columnNumber: 15
									}, this),
									/* @__PURE__ */ _jsxDEV("span", { children: "Mở Bản đồ Tác chiến GIS" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 75,
										columnNumber: 15
									}, this),
									/* @__PURE__ */ _jsxDEV(ArrowRight, { className: "w-4 h-4 ml-1" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 76,
										columnNumber: 15
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 70,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-center gap-2 px-4 py-2 rounded-lg bg-slate-800/80 border border-slate-700 text-xs font-mono text-slate-300",
								children: [/* @__PURE__ */ _jsxDEV(Database, { className: "w-4 h-4 text-emerald-400" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 79,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", { children: [totalUnits, " Điểm định vị không gian"] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 80,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 78,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 69,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 56,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "absolute right-0 top-0 bottom-0 w-96 opacity-15 pointer-events-none hidden md:block",
					children: /* @__PURE__ */ _jsxDEV("svg", {
						className: "w-full h-full",
						xmlns: "http://www.w3.org/2000/svg",
						children: [/* @__PURE__ */ _jsxDEV("defs", { children: /* @__PURE__ */ _jsxDEV("pattern", {
							id: "grid",
							width: "30",
							height: "30",
							patternUnits: "userSpaceOnUse",
							children: /* @__PURE__ */ _jsxDEV("path", {
								d: "M 30 0 L 0 0 0 30",
								fill: "none",
								stroke: "#10b981",
								strokeWidth: "1"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 90,
								columnNumber: 17
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 89,
							columnNumber: 15
						}, this) }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 88,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("rect", {
							width: "100%",
							height: "100%",
							fill: "url(#grid)"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 93,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 87,
						columnNumber: 11
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 86,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 55,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "Tổng số đơn vị quản lý"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 103,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-2xl font-bold text-white mt-1",
								children: totalUnits
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 104,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-emerald-400 mt-1 flex items-center gap-1",
								children: /* @__PURE__ */ _jsxDEV("span", { children: "Đầy đủ tọa độ VN-2000" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 106,
									columnNumber: 15
								}, this)
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 105,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 102,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "w-12 h-12 rounded-lg bg-emerald-950/60 border border-emerald-700/50 flex items-center justify-center text-emerald-400",
							children: /* @__PURE__ */ _jsxDEV(Building, { className: "w-6 h-6" }, void 0, false, {
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
						lineNumber: 101,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "SSCĐ Cấp 1 (Cao nhất)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 117,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-2xl font-bold text-rose-400 mt-1",
								children: level1Count
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 118,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-slate-400 mt-1",
								children: [
									"Tỷ lệ: ",
									Math.round(level1Count / totalUnits * 100),
									"% toàn thành phố"
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 119,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 116,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "w-12 h-12 rounded-lg bg-rose-950/60 border border-rose-700/50 flex items-center justify-center text-rose-400",
							children: /* @__PURE__ */ _jsxDEV(Activity, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 124,
								columnNumber: 13
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 123,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 115,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "SSCĐ Cấp 2 (Tăng cường)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 131,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-2xl font-bold text-amber-400 mt-1",
								children: level2Count
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 132,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-slate-400 mt-1",
								children: "Sẵn sàng chuyển trạng thái"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 133,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 130,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "w-12 h-12 rounded-lg bg-amber-950/60 border border-amber-700/50 flex items-center justify-center text-amber-400",
							children: /* @__PURE__ */ _jsxDEV(Shield, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 136,
								columnNumber: 13
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 135,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 129,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-xs text-slate-400 font-medium",
								children: "Độ phủ địa bàn"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 143,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-2xl font-bold text-sky-400 mt-1",
								children: "30 / 30"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 144,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[11px] text-slate-400 mt-1",
								children: "Quận, Huyện, Thị xã Hà Nội"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 145,
								columnNumber: 13
							}, this)
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 142,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "w-12 h-12 rounded-lg bg-sky-950/60 border border-sky-700/50 flex items-center justify-center text-sky-400",
							children: /* @__PURE__ */ _jsxDEV(MapPin, { className: "w-6 h-6" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 148,
								columnNumber: 13
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 147,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 141,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 99,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "grid grid-cols-1 lg:grid-cols-3 gap-6",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 shadow-lg space-y-4",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center justify-between border-b border-slate-800 pb-3",
						children: [/* @__PURE__ */ _jsxDEV("h3", {
							className: "text-sm font-bold text-white uppercase tracking-wider flex items-center gap-2",
							children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-4 h-4 text-emerald-400" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 159,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Phân bố theo Quân Binh Chủng" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 160,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 158,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-xs text-slate-400 font-mono",
							children: [sortedBranches.length, " nhóm"]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 162,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 157,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-3",
						children: sortedBranches.map(([branchName, count]) => {
							const percent = Math.round(count / totalUnits * 100);
							return /* @__PURE__ */ _jsxDEV("div", {
								onClick: () => {
									onFilterBranch(branchName);
									onNavigateToMap();
								},
								className: "group cursor-pointer p-2 rounded-lg hover:bg-slate-800/60 transition",
								children: [/* @__PURE__ */ _jsxDEV("div", {
									className: "flex justify-between text-xs mb-1",
									children: [/* @__PURE__ */ _jsxDEV("span", {
										className: "font-semibold text-slate-200 group-hover:text-emerald-300",
										children: branchName
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 178,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("span", {
										className: "font-mono text-slate-400",
										children: [
											count,
											" (",
											percent,
											"%)"
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 181,
										columnNumber: 21
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 177,
									columnNumber: 19
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "w-full h-2 bg-slate-800 rounded-full overflow-hidden",
									children: /* @__PURE__ */ _jsxDEV("div", {
										className: "h-full bg-emerald-500 rounded-full transition-all duration-500 group-hover:bg-emerald-400",
										style: { width: `${percent}%` }
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 186,
										columnNumber: 21
									}, this)
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 185,
									columnNumber: 19
								}, this)]
							}, branchName, true, {
								fileName: _jsxFileName,
								lineNumber: 169,
								columnNumber: 17
							}, this);
						})
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 165,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 156,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 shadow-lg space-y-4 lg:col-span-2",
					children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center justify-between border-b border-slate-800 pb-3",
							children: [/* @__PURE__ */ _jsxDEV("h3", {
								className: "text-sm font-bold text-white uppercase tracking-wider flex items-center gap-2",
								children: [/* @__PURE__ */ _jsxDEV(Award, { className: "w-4 h-4 text-amber-400" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 201,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Các Cơ quan Sở Chỉ huy & Đơn vị Trọng yếu" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 202,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 200,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-xs text-emerald-400 font-medium",
								children: "Bảo vệ Vùng lõi Thủ đô"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 204,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 199,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "grid grid-cols-1 md:grid-cols-2 gap-3",
							children: units.filter((u) => u.unitType === "Cơ quan Bộ Quốc phòng" || u.unitType === "Bộ Tư lệnh" || u.unitType === "Quân chủng" || u.id === "DV_MTA_01" || u.id === "DV_SD_361" || u.id === "DV_SD_301").slice(0, 6).map((u) => /* @__PURE__ */ _jsxDEV("div", {
								onClick: () => {
									onSelectUnit(u);
									onNavigateToMap();
								},
								className: "p-3 bg-[#0a1120] border border-slate-800 hover:border-emerald-600 rounded-lg cursor-pointer transition group shadow-sm flex flex-col justify-between",
								children: [/* @__PURE__ */ _jsxDEV("div", { children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center justify-between mb-1.5",
										children: [/* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] font-mono px-1.5 py-0.5 rounded bg-emerald-950 text-emerald-300 border border-emerald-800 font-semibold",
											children: u.code
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 230,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: `text-[10px] px-1.5 py-0.5 rounded font-medium ${u.readinessStatus.includes("Cấp 1") ? "bg-rose-950 text-rose-300" : "bg-emerald-950 text-emerald-300"}`,
											children: u.readinessStatus
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 233,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 229,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("h4", {
										className: "text-xs font-bold text-white group-hover:text-emerald-300 line-clamp-1",
										children: u.name
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 243,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("p", {
										className: "text-[11px] text-slate-400 mt-1 line-clamp-1 flex items-center gap-1",
										children: [/* @__PURE__ */ _jsxDEV(MapPin, { className: "w-3 h-3 text-sky-400 flex-shrink-0" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 247,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: u.address }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 248,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 246,
										columnNumber: 21
									}, this)
								] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 228,
									columnNumber: 19
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "mt-3 pt-2 border-t border-slate-800/80 flex justify-between items-center text-[10px] text-slate-400",
									children: [/* @__PURE__ */ _jsxDEV("span", { children: ["Chỉ huy: ", /* @__PURE__ */ _jsxDEV("strong", {
										className: "text-slate-200",
										children: u.commander.split("-")[0]
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 254,
										columnNumber: 32
									}, this)] }, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 253,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("span", {
										className: "text-emerald-400 group-hover:underline flex items-center gap-0.5 font-medium",
										children: "Xem vị trí →"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 256,
										columnNumber: 21
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 252,
									columnNumber: 19
								}, this)]
							}, u.id, true, {
								fileName: _jsxFileName,
								lineNumber: 220,
								columnNumber: 17
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 207,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "mt-4 p-3 rounded-lg bg-emerald-950/20 border border-emerald-800/30 text-xs text-slate-300 flex items-start gap-3",
							children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-5 h-5 text-emerald-400 flex-shrink-0 mt-0.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 266,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("strong", {
								className: "text-emerald-300",
								children: "Tính năng hệ quy chiếu VN-2000 chuẩn:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 268,
								columnNumber: 15
							}, this), " Mọi tọa độ địa lý WGS-84 (kinh độ, vĩ độ) đều được tự động chuyển đổi sang tọa độ phẳng UTM Zone 48N (X, Y tính bằng mét) theo đúng quy chuẩn quân sự và kỹ thuật bản đồ Việt Nam, cho phép tính toán cự ly cơ động, diện tích vùng đệm với độ chính xác đến hàng mét."] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 267,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 265,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 198,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 154,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 53,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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QkFBQyxRQUFELGFBQU0sYUFDSyx3QkFBQyxVQUFEO1VBQVEsV0FBVTtvQkFBa0IsRUFBRSxVQUFVLE1BQU0sR0FBRyxDQUFDLENBQUM7U0FBVzs7OztpQkFDM0U7Ozs7bUJBQ04sd0JBQUMsUUFBRDtVQUFNLFdBQVU7b0JBQStFO1NBRXpGOzs7O2lCQUNIOzs7OztnQkFDRjtVQXZDRSxFQUFFOzs7O2NBdUNKLENBQ047TUFDQTs7Ozs7TUFHTCx3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBZixDQUNFLHdCQUFDLFNBQUQsRUFBUyxXQUFVLGdEQUFpRDs7OztpQkFDcEUsd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFVBQUQ7UUFBUSxXQUFVO2tCQUFtQjtPQUE2Qzs7OztpQkFBQyx5UUFJaEY7Ozs7ZUFDRjs7Ozs7O0tBQ0Y7Ozs7O1lBQ0Y7Ozs7OztFQUNGOzs7Ozs7QUFFVCIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJEYXNoYm9hcmRWaWV3LnRzeCJdLCJ2ZXJzaW9uIjozLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgUmVhY3QgZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0IH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IEhBTk9JX0RJU1RSSUNUUyB9IGZyb20gJy4uL2RhdGEvaGFub2lEaXN0cmljdHMnO1xuaW1wb3J0IHtcbiAgU2hpZWxkLFxuICBBY3Rpdml0eSxcbiAgTGF5ZXJzLFxuICBNYXBQaW4sXG4gIENvbXBhc3MsXG4gIEFsZXJ0VHJpYW5nbGUsXG4gIEF3YXJkLFxuICBBcnJvd1JpZ2h0LFxuICBEYXRhYmFzZSxcbiAgQnVpbGRpbmcsXG59IGZyb20gJ2x1Y2lkZS1yZWFjdCc7XG5cbmludGVyZmFjZSBEYXNoYm9hcmRWaWV3UHJvcHMge1xuICB1bml0czogTWlsaXRhcnlVbml0W107XG4gIG9uTmF2aWdhdGVUb01hcDogKCkgPT4gdm9pZDtcbiAgb25GaWx0ZXJCcmFuY2g6IChicmFuY2g6IHN0cmluZykgPT4gdm9pZDtcbiAgb25TZWxlY3RVbml0OiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xufVxuXG5leHBvcnQgY29uc3QgRGFzaGJvYXJkVmlldzogUmVhY3QuRkM8RGFzaGJvYXJkVmlld1Byb3BzPiA9ICh7XG4gIHVuaXRzLFxuICBvbk5hdmlnYXRlVG9NYXAsXG4gIG9uRmlsdGVyQnJhbmNoLFxuICBvblNlbGVjdFVuaXQsXG59KSA9PiB7XG4gIC8vIENvbXB1dGUgYW5hbHl0aWNzXG4gIGNvbnN0IHRvdGFsVW5pdHMgPSB1bml0cy5sZW5ndGg7XG4gIGNvbnN0IGxldmVsMUNvdW50ID0gdW5pdHMuZmlsdGVyKCh1KSA9PiB1LnJlYWRpbmVzc1N0YXR1cy5pbmNsdWRlcygnQ+G6pXAgMScpKS5sZW5ndGg7XG4gIGNvbnN0IGxldmVsMkNvdW50ID0gdW5pdHMuZmlsdGVyKCh1KSA9PiB1LnJlYWRpbmVzc1N0YXR1cy5pbmNsdWRlcygnQ+G6pXAgMicpKS5sZW5ndGg7XG4gIGNvbnN0IHRyYWluaW5nQ291bnQgPSB1bml0cy5maWx0ZXIoKHUpID0+IHUucmVhZGluZXNzU3RhdHVzLmluY2x1ZGVzKCdIdeG6pW4gbHV54buHbicpKS5sZW5ndGg7XG5cbiAgLy8gQnJhbmNoIHN0YXRpc3RpY3NcbiAgY29uc3QgYnJhbmNoQ291bnRzID0gdW5pdHMucmVkdWNlKChhY2MsIHUpID0+IHtcbiAgICBhY2NbdS5icmFuY2hdID0gKGFjY1t1LmJyYW5jaF0gfHwgMCkgKyAxO1xuICAgIHJldHVybiBhY2M7XG4gIH0sIHt9IGFzIFJlY29yZDxzdHJpbmcsIG51bWJlcj4pO1xuXG4gIGNvbnN0IHNvcnRlZEJyYW5jaGVzID0gT2JqZWN0LmVudHJpZXMoYnJhbmNoQ291bnRzKS5zb3J0KChhLCBiKSA9PiBiWzFdIC0gYVsxXSk7XG5cbiAgLy8gRGlzdHJpY3QgZGlzdHJpYnV0aW9uXG4gIGNvbnN0IGRpc3RyaWN0Q291bnRzID0gdW5pdHMucmVkdWNlKChhY2MsIHUpID0+IHtcbiAgICBhY2NbdS5kaXN0cmljdF0gPSAoYWNjW3UuZGlzdHJpY3RdIHx8IDApICsgMTtcbiAgICByZXR1cm4gYWNjO1xuICB9LCB7fSBhcyBSZWNvcmQ8c3RyaW5nLCBudW1iZXI+KTtcblxuICBjb25zdCBzb3J0ZWREaXN0cmljdHMgPSBPYmplY3QuZW50cmllcyhkaXN0cmljdENvdW50cykuc29ydCgoYSwgYikgPT4gYlsxXSAtIGFbMV0pO1xuXG4gIHJldHVybiAoXG4gICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4LTEgb3ZlcmZsb3cteS1hdXRvIGJnLVsjMDgwZDE5XSB0ZXh0LXdoaXRlIHAtNiBzcGFjZS15LTZcIj5cbiAgICAgIHsvKiBUb3AgQmFubmVyICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJyZWxhdGl2ZSBvdmVyZmxvdy1oaWRkZW4gcm91bmRlZC14bCBiZy1ncmFkaWVudC10by1yIGZyb20tZW1lcmFsZC05NTAvNzAgdmlhLXNsYXRlLTkwMCB0by1zbGF0ZS05MDAgYm9yZGVyIGJvcmRlci1lbWVyYWxkLTgwMC80MCBwLTYgc2hhZG93LXhsXCI+XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicmVsYXRpdmUgei0xMCBtYXgtdy0zeGxcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImlubGluZS1mbGV4IGl0ZW1zLWNlbnRlciBnYXAtMiBweC0zIHB5LTEgcm91bmRlZC1mdWxsIGJnLWVtZXJhbGQtOTAwLzUwIGJvcmRlciBib3JkZXItZW1lcmFsZC01MDAvMzAgdGV4dC1lbWVyYWxkLTMwMCB0ZXh0LXhzIGZvbnQtc2VtaWJvbGQgbWItM1wiPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidy0yIGgtMiByb3VuZGVkLWZ1bGwgYmctZW1lcmFsZC00MDAgYW5pbWF0ZS1wdWxzZVwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5I4buHIHRo4buRbmcgQ8ahIHPhu58gZOG7ryBsaeG7h3UgS2jDtG5nIGdpYW4gR0lTIFF1w6JuIHPhu7E8L3NwYW4+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPGgyIGNsYXNzTmFtZT1cInRleHQtMnhsIGZvbnQtZXh0cmFib2xkIHRyYWNraW5nLXRpZ2h0IHRleHQtd2hpdGUgc206dGV4dC0zeGxcIj5cbiAgICAgICAgICAgIFThu5VuZyBxdWFuIEPGoSBz4bufIGThu68gbGnhu4d1IMSQxqFuIHbhu4sgUXXDom4gxJHhu5lpIHRyw6puIMSQ4buLYSBiw6BuIEjDoCBO4buZaVxuICAgICAgICAgIDwvaDI+XG4gICAgICAgICAgPHAgY2xhc3NOYW1lPVwibXQtMiB0ZXh0LXNtIHRleHQtc2xhdGUtMzAwIGxlYWRpbmctcmVsYXhlZFwiPlxuICAgICAgICAgICAgSOG7hyB0aOG7kW5nIGN1bmcgY+G6pXAgYuG7qWMgdHJhbmggdG/DoG4gY+G6o25oIHbhu4EgcGjDom4gYuG7kSBraMO0bmcgZ2lhbiwgdHLhuqFuZyB0aMOhaSBz4bq1biBzw6BuZyBjaGnhur9uXG4gICAgICAgICAgICDEkeG6pXUsIHThu41hIMSR4buZIGNodeG6qW4gVk4tMjAwMCAoRVBTRzozMjY0OCkgdsOgIGPDtG5nIGPhu6UgcGjDom4gdMOtY2ggdsO5bmcgxJHhu4dtIHTDoWMgY2hp4bq/biBj4bunYSBjw6FjXG4gICAgICAgICAgICDEkcahbiB24buLIHF1w6JuIMSR4buZaSB0csOqbiAzMCBxdeG6rW4sIGh1eeG7h24sIHRo4buLIHjDoyB0aHXhu5ljIHRow6BuaCBwaOG7kSBIw6AgTuG7mWkuXG4gICAgICAgICAgPC9wPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwibXQtNSBmbGV4IGZsZXgtd3JhcCBnYXAtM1wiPlxuICAgICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgICBvbkNsaWNrPXtvbk5hdmlnYXRlVG9NYXB9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLWVtZXJhbGQtNjAwIGhvdmVyOmJnLWVtZXJhbGQtNTAwIHRleHQtd2hpdGUgZm9udC1zZW1pYm9sZCB0ZXh0LXhzIHB4LTQgcHktMi41IHJvdW5kZWQtbGcgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTIgc2hhZG93LWxnIHRyYW5zaXRpb24gYWN0aXZlOnNjYWxlLTk1XCJcbiAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgPENvbXBhc3MgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuPk3hu58gQuG6o24gxJHhu5MgVMOhYyBjaGnhur9uIEdJUzwvc3Bhbj5cbiAgICAgICAgICAgICAgPEFycm93UmlnaHQgY2xhc3NOYW1lPVwidy00IGgtNCBtbC0xXCIgLz5cbiAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMiBweC00IHB5LTIgcm91bmRlZC1sZyBiZy1zbGF0ZS04MDAvODAgYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgdGV4dC14cyBmb250LW1vbm8gdGV4dC1zbGF0ZS0zMDBcIj5cbiAgICAgICAgICAgICAgPERhdGFiYXNlIGNsYXNzTmFtZT1cInctNCBoLTQgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuPnt0b3RhbFVuaXRzfSDEkGnhu4NtIMSR4buLbmggduG7iyBraMO0bmcgZ2lhbjwvc3Bhbj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogRGVjb3JhdGl2ZSBHcmlkIFNWRyBpbiBiYWNrZ3JvdW5kICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImFic29sdXRlIHJpZ2h0LTAgdG9wLTAgYm90dG9tLTAgdy05NiBvcGFjaXR5LTE1IHBvaW50ZXItZXZlbnRzLW5vbmUgaGlkZGVuIG1kOmJsb2NrXCI+XG4gICAgICAgICAgPHN2ZyBjbGFzc05hbWU9XCJ3LWZ1bGwgaC1mdWxsXCIgeG1sbnM9XCJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2Z1wiPlxuICAgICAgICAgICAgPGRlZnM+XG4gICAgICAgICAgICAgIDxwYXR0ZXJuIGlkPVwiZ3JpZFwiIHdpZHRoPVwiMzBcIiBoZWlnaHQ9XCIzMFwiIHBhdHRlcm5Vbml0cz1cInVzZXJTcGFjZU9uVXNlXCI+XG4gICAgICAgICAgICAgICAgPHBhdGggZD1cIk0gMzAgMCBMIDAgMCAwIDMwXCIgZmlsbD1cIm5vbmVcIiBzdHJva2U9XCIjMTBiOTgxXCIgc3Ryb2tlV2lkdGg9XCIxXCIgLz5cbiAgICAgICAgICAgICAgPC9wYXR0ZXJuPlxuICAgICAgICAgICAgPC9kZWZzPlxuICAgICAgICAgICAgPHJlY3Qgd2lkdGg9XCIxMDAlXCIgaGVpZ2h0PVwiMTAwJVwiIGZpbGw9XCJ1cmwoI2dyaWQpXCIgLz5cbiAgICAgICAgICA8L3N2Zz5cbiAgICAgICAgPC9kaXY+XG4gICAgICA8L2Rpdj5cblxuICAgICAgey8qIEtQSSBTdGF0IENhcmRzICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIHNtOmdyaWQtY29scy0yIGxnOmdyaWQtY29scy00IGdhcC00XCI+XG4gICAgICAgIHsvKiBUb3RhbCBVbml0cyAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC14bCBwLTQgc2hhZG93LW1kIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlblwiPlxuICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDAgZm9udC1tZWRpdW1cIj5U4buVbmcgc+G7kSDEkcahbiB24buLIHF14bqjbiBsw708L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC0yeGwgZm9udC1ib2xkIHRleHQtd2hpdGUgbXQtMVwiPnt0b3RhbFVuaXRzfTwvZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSB0ZXh0LWVtZXJhbGQtNDAwIG10LTEgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTFcIj5cbiAgICAgICAgICAgICAgPHNwYW4+xJDhuqd5IMSR4bunIHThu41hIMSR4buZIFZOLTIwMDA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInctMTIgaC0xMiByb3VuZGVkLWxnIGJnLWVtZXJhbGQtOTUwLzYwIGJvcmRlciBib3JkZXItZW1lcmFsZC03MDAvNTAgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdGV4dC1lbWVyYWxkLTQwMFwiPlxuICAgICAgICAgICAgPEJ1aWxkaW5nIGNsYXNzTmFtZT1cInctNiBoLTZcIiAvPlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogTGV2ZWwgMSBSZWFkaW5lc3MgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC00IHNoYWRvdy1tZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW5cIj5cbiAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGZvbnQtbWVkaXVtXCI+U1NDxJAgQ+G6pXAgMSAoQ2FvIG5o4bqldCk8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC0yeGwgZm9udC1ib2xkIHRleHQtcm9zZS00MDAgbXQtMVwiPntsZXZlbDFDb3VudH08L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlxuICAgICAgICAgICAgICBU4bu3IGzhu4c6IHtNYXRoLnJvdW5kKChsZXZlbDFDb3VudCAvIHRvdGFsVW5pdHMpICogMTAwKX0lIHRvw6BuIHRow6BuaCBwaOG7kVxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ3LTEyIGgtMTIgcm91bmRlZC1sZyBiZy1yb3NlLTk1MC82MCBib3JkZXIgYm9yZGVyLXJvc2UtNzAwLzUwIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIHRleHQtcm9zZS00MDBcIj5cbiAgICAgICAgICAgIDxBY3Rpdml0eSBjbGFzc05hbWU9XCJ3LTYgaC02XCIgLz5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgey8qIExldmVsIDIgUmVhZGluZXNzICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIHAtNCBzaGFkb3ctbWQgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuXCI+XG4gICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LXNsYXRlLTQwMCBmb250LW1lZGl1bVwiPlNTQ8SQIEPhuqVwIDIgKFTEg25nIGPGsOG7nW5nKTwvZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LTJ4bCBmb250LWJvbGQgdGV4dC1hbWJlci00MDAgbXQtMVwiPntsZXZlbDJDb3VudH08L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlPhurVuIHPDoG5nIGNodXnhu4NuIHRy4bqhbmcgdGjDoWk8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInctMTIgaC0xMiByb3VuZGVkLWxnIGJnLWFtYmVyLTk1MC82MCBib3JkZXIgYm9yZGVyLWFtYmVyLTcwMC81MCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0ZXh0LWFtYmVyLTQwMFwiPlxuICAgICAgICAgICAgPFNoaWVsZCBjbGFzc05hbWU9XCJ3LTYgaC02XCIgLz5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgey8qIEFkbWluaXN0cmF0aXZlIENvdmVyYWdlICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIHAtNCBzaGFkb3ctbWQgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuXCI+XG4gICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LXNsYXRlLTQwMCBmb250LW1lZGl1bVwiPsSQ4buZIHBo4bunIMSR4buLYSBiw6BuPC9kaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtMnhsIGZvbnQtYm9sZCB0ZXh0LXNreS00MDAgbXQtMVwiPjMwIC8gMzA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlF14bqtbiwgSHV54buHbiwgVGjhu4sgeMOjIEjDoCBO4buZaTwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidy0xMiBoLTEyIHJvdW5kZWQtbGcgYmctc2t5LTk1MC82MCBib3JkZXIgYm9yZGVyLXNreS03MDAvNTAgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdGV4dC1za3ktNDAwXCI+XG4gICAgICAgICAgICA8TWFwUGluIGNsYXNzTmFtZT1cInctNiBoLTZcIiAvPlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cbiAgICAgIDwvZGl2PlxuXG4gICAgICB7LyogTWFpbiBHcmlkOiBCcmFuY2hlcyBCcmVha2Rvd24gJiBTdHJhdGVnaWMgVW5pdHMgbGlzdCAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMSBsZzpncmlkLWNvbHMtMyBnYXAtNlwiPlxuICAgICAgICB7LyogTGVmdDogQnJhbmNoIERpc3RyaWJ1dGlvbiAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC14bCBwLTUgc2hhZG93LWxnIHNwYWNlLXktNFwiPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGJvcmRlci1iIGJvcmRlci1zbGF0ZS04MDAgcGItM1wiPlxuICAgICAgICAgICAgPGgzIGNsYXNzTmFtZT1cInRleHQtc20gZm9udC1ib2xkIHRleHQtd2hpdGUgdXBwZXJjYXNlIHRyYWNraW5nLXdpZGVyIGZsZXggaXRlbXMtY2VudGVyIGdhcC0yXCI+XG4gICAgICAgICAgICAgIDxMYXllcnMgY2xhc3NOYW1lPVwidy00IGgtNCB0ZXh0LWVtZXJhbGQtNDAwXCIgLz5cbiAgICAgICAgICAgICAgPHNwYW4+UGjDom4gYuG7kSB0aGVvIFF1w6JuIEJpbmggQ2jhu6duZzwvc3Bhbj5cbiAgICAgICAgICAgIDwvaDM+XG4gICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGZvbnQtbW9ub1wiPntzb3J0ZWRCcmFuY2hlcy5sZW5ndGh9IG5ow7NtPC9zcGFuPlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTNcIj5cbiAgICAgICAgICAgIHtzb3J0ZWRCcmFuY2hlcy5tYXAoKFticmFuY2hOYW1lLCBjb3VudF0pID0+IHtcbiAgICAgICAgICAgICAgY29uc3QgcGVyY2VudCA9IE1hdGgucm91bmQoKGNvdW50IC8gdG90YWxVbml0cykgKiAxMDApO1xuICAgICAgICAgICAgICByZXR1cm4gKFxuICAgICAgICAgICAgICAgIDxkaXZcbiAgICAgICAgICAgICAgICAgIGtleT17YnJhbmNoTmFtZX1cbiAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHtcbiAgICAgICAgICAgICAgICAgICAgb25GaWx0ZXJCcmFuY2goYnJhbmNoTmFtZSk7XG4gICAgICAgICAgICAgICAgICAgIG9uTmF2aWdhdGVUb01hcCgpO1xuICAgICAgICAgICAgICAgICAgfX1cbiAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cImdyb3VwIGN1cnNvci1wb2ludGVyIHAtMiByb3VuZGVkLWxnIGhvdmVyOmJnLXNsYXRlLTgwMC82MCB0cmFuc2l0aW9uXCJcbiAgICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXgganVzdGlmeS1iZXR3ZWVuIHRleHQteHMgbWItMVwiPlxuICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LXNlbWlib2xkIHRleHQtc2xhdGUtMjAwIGdyb3VwLWhvdmVyOnRleHQtZW1lcmFsZC0zMDBcIj5cbiAgICAgICAgICAgICAgICAgICAgICB7YnJhbmNoTmFtZX1cbiAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC1zbGF0ZS00MDBcIj5cbiAgICAgICAgICAgICAgICAgICAgICB7Y291bnR9ICh7cGVyY2VudH0lKVxuICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidy1mdWxsIGgtMiBiZy1zbGF0ZS04MDAgcm91bmRlZC1mdWxsIG92ZXJmbG93LWhpZGRlblwiPlxuICAgICAgICAgICAgICAgICAgICA8ZGl2XG4gICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiaC1mdWxsIGJnLWVtZXJhbGQtNTAwIHJvdW5kZWQtZnVsbCB0cmFuc2l0aW9uLWFsbCBkdXJhdGlvbi01MDAgZ3JvdXAtaG92ZXI6YmctZW1lcmFsZC00MDBcIlxuICAgICAgICAgICAgICAgICAgICAgIHN0eWxlPXt7IHdpZHRoOiBgJHtwZXJjZW50fSVgIH19XG4gICAgICAgICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgIH0pfVxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogQ2VudGVyOiBUb3AgU3RyYXRlZ2ljIEhlYWRxdWFydGVycyAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC14bCBwLTUgc2hhZG93LWxnIHNwYWNlLXktNCBsZzpjb2wtc3Bhbi0yXCI+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi0zXCI+XG4gICAgICAgICAgICA8aDMgY2xhc3NOYW1lPVwidGV4dC1zbSBmb250LWJvbGQgdGV4dC13aGl0ZSB1cHBlcmNhc2UgdHJhY2tpbmctd2lkZXIgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgICAgPEF3YXJkIGNsYXNzTmFtZT1cInctNCBoLTQgdGV4dC1hbWJlci00MDBcIiAvPlxuICAgICAgICAgICAgICA8c3Bhbj5Dw6FjIEPGoSBxdWFuIFPhu58gQ2jhu4kgaHV5ICYgxJDGoW4gduG7iyBUcuG7jW5nIHnhur91PC9zcGFuPlxuICAgICAgICAgICAgPC9oMz5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1lbWVyYWxkLTQwMCBmb250LW1lZGl1bVwiPkLhuqNvIHbhu4cgVsO5bmcgbMO1aSBUaOG7pyDEkcO0PC9zcGFuPlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIG1kOmdyaWQtY29scy0yIGdhcC0zXCI+XG4gICAgICAgICAgICB7dW5pdHNcbiAgICAgICAgICAgICAgLmZpbHRlcihcbiAgICAgICAgICAgICAgICAodSkgPT5cbiAgICAgICAgICAgICAgICAgIHUudW5pdFR5cGUgPT09ICdDxqEgcXVhbiBC4buZIFF14buRYyBwaMOybmcnIHx8XG4gICAgICAgICAgICAgICAgICB1LnVuaXRUeXBlID09PSAnQuG7mSBUxrAgbOG7h25oJyB8fFxuICAgICAgICAgICAgICAgICAgdS51bml0VHlwZSA9PT0gJ1F1w6JuIGNo4bunbmcnIHx8XG4gICAgICAgICAgICAgICAgICB1LmlkID09PSAnRFZfTVRBXzAxJyB8fFxuICAgICAgICAgICAgICAgICAgdS5pZCA9PT0gJ0RWX1NEXzM2MScgfHxcbiAgICAgICAgICAgICAgICAgIHUuaWQgPT09ICdEVl9TRF8zMDEnXG4gICAgICAgICAgICAgIClcbiAgICAgICAgICAgICAgLnNsaWNlKDAsIDYpXG4gICAgICAgICAgICAgIC5tYXAoKHUpID0+IChcbiAgICAgICAgICAgICAgICA8ZGl2XG4gICAgICAgICAgICAgICAgICBrZXk9e3UuaWR9XG4gICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIG9uU2VsZWN0VW5pdCh1KTtcbiAgICAgICAgICAgICAgICAgICAgb25OYXZpZ2F0ZVRvTWFwKCk7XG4gICAgICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwicC0zIGJnLVsjMGExMTIwXSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCBob3Zlcjpib3JkZXItZW1lcmFsZC02MDAgcm91bmRlZC1sZyBjdXJzb3ItcG9pbnRlciB0cmFuc2l0aW9uIGdyb3VwIHNoYWRvdy1zbSBmbGV4IGZsZXgtY29sIGp1c3RpZnktYmV0d2VlblwiXG4gICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gbWItMS41XCI+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gZm9udC1tb25vIHB4LTEuNSBweS0wLjUgcm91bmRlZCBiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwIGJvcmRlciBib3JkZXItZW1lcmFsZC04MDAgZm9udC1zZW1pYm9sZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAge3UuY29kZX1cbiAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW5cbiAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT17YHRleHQtWzEwcHhdIHB4LTEuNSBweS0wLjUgcm91bmRlZCBmb250LW1lZGl1bSAke1xuICAgICAgICAgICAgICAgICAgICAgICAgICB1LnJlYWRpbmVzc1N0YXR1cy5pbmNsdWRlcygnQ+G6pXAgMScpXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPyAnYmctcm9zZS05NTAgdGV4dC1yb3NlLTMwMCdcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA6ICdiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwJ1xuICAgICAgICAgICAgICAgICAgICAgICAgfWB9XG4gICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAge3UucmVhZGluZXNzU3RhdHVzfVxuICAgICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDxoNCBjbGFzc05hbWU9XCJ0ZXh0LXhzIGZvbnQtYm9sZCB0ZXh0LXdoaXRlIGdyb3VwLWhvdmVyOnRleHQtZW1lcmFsZC0zMDAgbGluZS1jbGFtcC0xXCI+XG4gICAgICAgICAgICAgICAgICAgICAge3UubmFtZX1cbiAgICAgICAgICAgICAgICAgICAgPC9oND5cbiAgICAgICAgICAgICAgICAgICAgPHAgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMSBsaW5lLWNsYW1wLTEgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8TWFwUGluIGNsYXNzTmFtZT1cInctMyBoLTMgdGV4dC1za3ktNDAwIGZsZXgtc2hyaW5rLTBcIiAvPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuPnt1LmFkZHJlc3N9PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICA8L3A+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJtdC0zIHB0LTIgYm9yZGVyLXQgYm9yZGVyLXNsYXRlLTgwMC84MCBmbGV4IGp1c3RpZnktYmV0d2VlbiBpdGVtcy1jZW50ZXIgdGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDBcIj5cbiAgICAgICAgICAgICAgICAgICAgPHNwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgQ2jhu4kgaHV5OiA8c3Ryb25nIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtMjAwXCI+e3UuY29tbWFuZGVyLnNwbGl0KCctJylbMF19PC9zdHJvbmc+XG4gICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1lbWVyYWxkLTQwMCBncm91cC1ob3Zlcjp1bmRlcmxpbmUgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTAuNSBmb250LW1lZGl1bVwiPlxuICAgICAgICAgICAgICAgICAgICAgIFhlbSB24buLIHRyw60g4oaSXG4gICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICApKX1cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBRdWljayBHSVMgRmVhdHVyZXMgRXhwbGFuYXRpb24gKi99XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJtdC00IHAtMyByb3VuZGVkLWxnIGJnLWVtZXJhbGQtOTUwLzIwIGJvcmRlciBib3JkZXItZW1lcmFsZC04MDAvMzAgdGV4dC14cyB0ZXh0LXNsYXRlLTMwMCBmbGV4IGl0ZW1zLXN0YXJ0IGdhcC0zXCI+XG4gICAgICAgICAgICA8Q29tcGFzcyBjbGFzc05hbWU9XCJ3LTUgaC01IHRleHQtZW1lcmFsZC00MDAgZmxleC1zaHJpbmstMCBtdC0wLjVcIiAvPlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPHN0cm9uZyBjbGFzc05hbWU9XCJ0ZXh0LWVtZXJhbGQtMzAwXCI+VMOtbmggbsSDbmcgaOG7hyBxdXkgY2hp4bq/dSBWTi0yMDAwIGNodeG6qW46PC9zdHJvbmc+IE3hu41pXG4gICAgICAgICAgICAgIHThu41hIMSR4buZIMSR4buLYSBsw70gV0dTLTg0IChraW5oIMSR4buZLCB2xKkgxJHhu5kpIMSR4buBdSDEkcaw4bujYyB04buxIMSR4buZbmcgY2h1eeG7g24gxJHhu5VpIHNhbmcgdOG7jWEgxJHhu5kgcGjhurNuZyBVVE1cbiAgICAgICAgICAgICAgWm9uZSA0OE4gKFgsIFkgdMOtbmggYuG6sW5nIG3DqXQpIHRoZW8gxJHDum5nIHF1eSBjaHXhuqluIHF1w6JuIHPhu7EgdsOgIGvhu7kgdGh14bqtdCBi4bqjbiDEkeG7kyBWaeG7h3QgTmFtLFxuICAgICAgICAgICAgICBjaG8gcGjDqXAgdMOtbmggdG/DoW4gY+G7sSBseSBjxqEgxJHhu5luZywgZGnhu4duIHTDrWNoIHbDuW5nIMSR4buHbSB24bubaSDEkeG7mSBjaMOtbmggeMOhYyDEkeG6v24gaMOgbmcgbcOpdC5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cbiAgICAgIDwvZGl2PlxuICAgIDwvZGl2PlxuICApO1xufTtcbiJdfQ==