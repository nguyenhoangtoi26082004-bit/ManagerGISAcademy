const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"];const _jsxDEV = __vite__cjsImport2_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { BarChart3, Printer, Activity, Layers, FileText } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/StatisticsView.tsx";
import __vite__cjsImport2_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const StatisticsView = ({ units }) => {
	const [printMode, setPrintMode] = useState(false);
	const total = units.length;
	// Breakdown by branch
	const branchCounts = units.reduce((acc, u) => {
		acc[u.branch] = (acc[u.branch] || 0) + 1;
		return acc;
	}, {});
	// Breakdown by readiness
	const readinessCounts = units.reduce((acc, u) => {
		acc[u.readinessStatus] = (acc[u.readinessStatus] || 0) + 1;
		return acc;
	}, {});
	// Breakdown by security level
	const securityCounts = units.reduce((acc, u) => {
		acc[u.securityLevel] = (acc[u.securityLevel] || 0) + 1;
		return acc;
	}, {});
	// Breakdown by unit type
	const typeCounts = units.reduce((acc, u) => {
		acc[u.unitType] = (acc[u.unitType] || 0) + 1;
		return acc;
	}, {});
	const handlePrint = () => {
		setPrintMode(true);
		setTimeout(() => {
			window.print();
			setPrintMode(false);
		}, 200);
	};
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex-1 overflow-y-auto bg-[#080d19] text-white p-6 space-y-6",
		children: [/* @__PURE__ */ _jsxDEV("div", {
			className: "flex flex-wrap items-center justify-between gap-4 border-b border-slate-800 pb-4",
			children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h2", {
				className: "text-xl font-bold uppercase tracking-wider text-white flex items-center gap-2",
				children: [/* @__PURE__ */ _jsxDEV(BarChart3, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 63,
					columnNumber: 13
				}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Thống kê & Lập Báo cáo Tổng hợp GIS" }, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 64,
					columnNumber: 13
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 62,
				columnNumber: 11
			}, this), /* @__PURE__ */ _jsxDEV("p", {
				className: "text-xs text-slate-400 mt-1",
				children: "Phân tích số liệu thống kê lực lượng, cơ cấu quân binh chủng và xuất văn bản báo cáo kỹ thuật"
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 66,
				columnNumber: 11
			}, this)] }, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 61,
				columnNumber: 9
			}, this), /* @__PURE__ */ _jsxDEV("div", {
				className: "flex items-center gap-3",
				children: /* @__PURE__ */ _jsxDEV("button", {
					onClick: handlePrint,
					className: "bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold px-4 py-2 rounded-lg flex items-center gap-2 shadow transition",
					children: [/* @__PURE__ */ _jsxDEV(Printer, { className: "w-4 h-4" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 77,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "In Báo Cáo Kỹ Thuật (PDF)" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 78,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 73,
					columnNumber: 11
				}, this)
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 72,
				columnNumber: 9
			}, this)]
		}, void 0, true, {
			fileName: _jsxFileName,
			lineNumber: 60,
			columnNumber: 7
		}, this), /* @__PURE__ */ _jsxDEV("div", {
			className: "bg-[#0e1628] border border-slate-800 rounded-xl p-6 shadow-xl space-y-6",
			children: [
				/* @__PURE__ */ _jsxDEV("div", {
					className: "text-center space-y-1 border-b border-slate-700/80 pb-4",
					children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "text-xs uppercase font-semibold tracking-wider text-slate-300",
							children: "HỌC VIỆN KỸ THUẬT QUÂN SỰ - BỘ QUỐC PHÒNG"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 87,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "text-[11px] uppercase font-medium text-slate-400",
							children: "HỆ THỐNG CƠ SỞ DỮ LIỆU GIS QUẢN LÝ ĐƠN VỊ QUÂN ĐỘI HÀ NỘI"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 90,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", { className: "w-32 h-0.5 bg-emerald-500 mx-auto my-2" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 93,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("h1", {
							className: "text-lg font-bold text-white uppercase tracking-wider",
							children: "BÁO CÁO TỔNG HỢP DỮ LIỆU KHÔNG GIAN GIS CÁC ĐƠN VỊ TRÊN ĐỊA BÀN THỦ ĐÔ HÀ NỘI"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 94,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("p", {
							className: "text-xs text-slate-400",
							children: "(Hệ tọa độ VN-2000 / UTM Zone 48N - EPSG:32648 & WGS-84 - EPSG:4326)"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 97,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 86,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "grid grid-cols-2 md:grid-cols-4 gap-4",
					children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "bg-[#090f1d] border border-slate-800 p-3 rounded-lg text-center",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[10px] text-slate-400 uppercase font-bold block",
								children: "Tổng số đơn vị"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 105,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-2xl font-bold text-emerald-400 font-mono",
								children: total
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 108,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 104,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "bg-[#090f1d] border border-slate-800 p-3 rounded-lg text-center",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[10px] text-slate-400 uppercase font-bold block",
								children: "SSCĐ Cấp 1"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 111,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-2xl font-bold text-rose-400 font-mono",
								children: readinessCounts["Cấp 1 (Cao nhất)"] || 0
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 114,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 110,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "bg-[#090f1d] border border-slate-800 p-3 rounded-lg text-center",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[10px] text-slate-400 uppercase font-bold block",
								children: "Số Quân Binh Chủng"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 119,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-2xl font-bold text-sky-400 font-mono",
								children: Object.keys(branchCounts).length
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 122,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 118,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "bg-[#090f1d] border border-slate-800 p-3 rounded-lg text-center",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[10px] text-slate-400 uppercase font-bold block",
								children: "Độ phủ quận/huyện"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 127,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-2xl font-bold text-amber-400 font-mono",
								children: "30 / 30"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 130,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 126,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 103,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "grid grid-cols-1 md:grid-cols-2 gap-6 pt-2",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-3 bg-[#090f1d] p-4 rounded-xl border border-slate-800",
						children: [/* @__PURE__ */ _jsxDEV("h3", {
							className: "text-xs font-bold uppercase tracking-wider text-emerald-400 flex items-center gap-1.5",
							children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 139,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Cơ cấu Quân Binh Chủng" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 140,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 138,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "space-y-2",
							children: Object.entries(branchCounts).map(([branch, count]) => {
								const pct = Math.round(count / total * 100);
								return /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("div", {
									className: "flex justify-between text-xs mb-1",
									children: [/* @__PURE__ */ _jsxDEV("span", {
										className: "text-slate-300 font-medium",
										children: branch
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 148,
										columnNumber: 23
									}, this), /* @__PURE__ */ _jsxDEV("span", {
										className: "text-slate-400 font-mono",
										children: [
											count,
											" (",
											pct,
											"%)"
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 149,
										columnNumber: 23
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 147,
									columnNumber: 21
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "w-full h-1.5 bg-slate-800 rounded-full overflow-hidden",
									children: /* @__PURE__ */ _jsxDEV("div", {
										className: "h-full bg-emerald-500 rounded-full",
										style: { width: `${pct}%` }
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 154,
										columnNumber: 23
									}, this)
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 153,
									columnNumber: 21
								}, this)] }, branch, true, {
									fileName: _jsxFileName,
									lineNumber: 146,
									columnNumber: 19
								}, this);
							})
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 142,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 137,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-3 bg-[#090f1d] p-4 rounded-xl border border-slate-800",
						children: [
							/* @__PURE__ */ _jsxDEV("h3", {
								className: "text-xs font-bold uppercase tracking-wider text-amber-400 flex items-center gap-1.5",
								children: [/* @__PURE__ */ _jsxDEV(Activity, { className: "w-3.5 h-3.5" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 168,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Trạng thái Sẵn sàng Chiến đấu (SSCĐ)" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 169,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 167,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "space-y-2",
								children: Object.entries(readinessCounts).map(([status, count]) => {
									const pct = Math.round(count / total * 100);
									const color = status.includes("Cấp 1") ? "bg-rose-500" : status.includes("Cấp 2") ? "bg-amber-500" : "bg-emerald-500";
									return /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("div", {
										className: "flex justify-between text-xs mb-1",
										children: [/* @__PURE__ */ _jsxDEV("span", {
											className: "text-slate-300 font-medium",
											children: status
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 183,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-slate-400 font-mono",
											children: [
												count,
												" (",
												pct,
												"%)"
											]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 184,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 182,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("div", {
										className: "w-full h-1.5 bg-slate-800 rounded-full overflow-hidden",
										children: /* @__PURE__ */ _jsxDEV("div", {
											className: `h-full rounded-full ${color}`,
											style: { width: `${pct}%` }
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 189,
											columnNumber: 23
										}, this)
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 188,
										columnNumber: 21
									}, this)] }, status, true, {
										fileName: _jsxFileName,
										lineNumber: 181,
										columnNumber: 19
									}, this);
								})
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 171,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "pt-4 mt-4 border-t border-slate-800 space-y-2",
								children: [/* @__PURE__ */ _jsxDEV("h4", {
									className: "text-[11px] font-bold uppercase text-slate-400",
									children: "Phân loại cấp độ bảo mật CSDL:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 201,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "grid grid-cols-2 gap-2 text-xs",
									children: Object.entries(securityCounts).map(([sec, count]) => /* @__PURE__ */ _jsxDEV("div", {
										className: "p-2 rounded bg-slate-800/60 border border-slate-700/60 flex justify-between",
										children: [/* @__PURE__ */ _jsxDEV("span", {
											className: "text-slate-300",
											children: [sec, ":"]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 210,
											columnNumber: 21
										}, this), /* @__PURE__ */ _jsxDEV("strong", {
											className: "text-emerald-400 font-mono",
											children: count
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 211,
											columnNumber: 21
										}, this)]
									}, sec, true, {
										fileName: _jsxFileName,
										lineNumber: 206,
										columnNumber: 19
									}, this))
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 204,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 200,
								columnNumber: 13
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 166,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 135,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "space-y-3",
					children: [/* @__PURE__ */ _jsxDEV("h3", {
						className: "text-xs font-bold uppercase tracking-wider text-white flex items-center gap-1.5",
						children: [/* @__PURE__ */ _jsxDEV(FileText, { className: "w-3.5 h-3.5 text-sky-400" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 222,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Danh mục Tổng hợp các Đơn vị Quân sự đã Số hóa" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 223,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 221,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "overflow-x-auto border border-slate-800 rounded-lg",
						children: /* @__PURE__ */ _jsxDEV("table", {
							className: "w-full text-left text-xs text-slate-300",
							children: [/* @__PURE__ */ _jsxDEV("thead", {
								className: "bg-[#090f1d] text-slate-400 uppercase text-[10px] font-mono border-b border-slate-800",
								children: /* @__PURE__ */ _jsxDEV("tr", { children: [
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "TT"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 230,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Mã QS"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 231,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Tên Đơn vị"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 232,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Binh chủng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 233,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Quận / Huyện"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 234,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "X (VN-2000 m)"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 235,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Y (VN-2000 m)"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 236,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Trực ban"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 237,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-2.5",
										children: "Trạng thái SSCĐ"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 238,
										columnNumber: 19
									}, this)
								] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 229,
									columnNumber: 17
								}, this)
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 228,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("tbody", {
								className: "divide-y divide-slate-800/80 font-sans",
								children: units.map((u, idx) => /* @__PURE__ */ _jsxDEV("tr", {
									className: "hover:bg-slate-800/40",
									children: [
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5 font-mono text-slate-400",
											children: idx + 1
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 244,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5 font-mono font-bold text-emerald-400",
											children: u.code
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 245,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5 font-semibold text-white",
											children: u.name
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 246,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5",
											children: u.branch
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 247,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5",
											children: u.district
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 248,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5 font-mono text-sky-300",
											children: u.x_vn2000.toLocaleString()
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 249,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5 font-mono text-sky-300",
											children: u.y_vn2000.toLocaleString()
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 250,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5 font-mono text-emerald-300",
											children: u.phone
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 251,
											columnNumber: 21
										}, this),
										/* @__PURE__ */ _jsxDEV("td", {
											className: "p-2.5",
											children: u.readinessStatus
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 252,
											columnNumber: 21
										}, this)
									]
								}, u.id, true, {
									fileName: _jsxFileName,
									lineNumber: 243,
									columnNumber: 19
								}, this))
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 241,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 227,
							columnNumber: 13
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 226,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 220,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "pt-8 border-t border-slate-800 grid grid-cols-2 text-center text-xs text-slate-300",
					children: [/* @__PURE__ */ _jsxDEV("div", { children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "font-bold uppercase text-white",
							children: "NGƯỜI LẬP BÁO CÁO"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 263,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "text-[11px] text-slate-400 mt-0.5",
							children: "Kỹ sư Trắc địa - Bản đồ & GIS"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 264,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", { className: "h-16" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 265,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "font-semibold text-white",
							children: "(Ký và ghi rõ họ tên)"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 266,
							columnNumber: 13
						}, this)
					] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 262,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("div", { children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "font-bold uppercase text-white",
							children: "THỦ TRƯỞNG PHÊ DUYỆT"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 269,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "text-[11px] text-slate-400 mt-0.5",
							children: "Sở Chỉ huy / Cơ quan Tác chiến"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 270,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", { className: "h-16" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 271,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "font-semibold text-white",
							children: "(Ký tên và đóng dấu)"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 272,
							columnNumber: 13
						}, this)
					] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 268,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 261,
					columnNumber: 9
				}, this)
			]
		}, void 0, true, {
			fileName: _jsxFileName,
			lineNumber: 84,
			columnNumber: 7
		}, this)]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 58,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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Vzs7Ozs7VUFDcEMsd0JBQUMsTUFBRDtXQUFJLFdBQVU7cUJBQVMsRUFBRTtVQUFhOzs7OztVQUN0Qyx3QkFBQyxNQUFEO1dBQUksV0FBVTtxQkFBZ0MsRUFBRSxTQUFTLGVBQWU7VUFBTTs7Ozs7VUFDOUUsd0JBQUMsTUFBRDtXQUFJLFdBQVU7cUJBQWdDLEVBQUUsU0FBUyxlQUFlO1VBQU07Ozs7O1VBQzlFLHdCQUFDLE1BQUQ7V0FBSSxXQUFVO3FCQUFvQyxFQUFFO1VBQVU7Ozs7O1VBQzlELHdCQUFDLE1BQUQ7V0FBSSxXQUFVO3FCQUFTLEVBQUU7VUFBb0I7Ozs7O1NBQzNDO1dBVkssRUFBRTs7OztlQVVQLENBQ0w7T0FDSTs7OztlQUNGOzs7Ozs7S0FDSjs7OzthQUNGOzs7Ozs7SUFHTCx3QkFBQyxPQUFEO0tBQUssV0FBVTtlQUFmLENBQ0Usd0JBQUMsT0FBRDtNQUNFLHdCQUFDLE9BQUQ7T0FBSyxXQUFVO2lCQUFpQztNQUFzQjs7Ozs7TUFDdEUsd0JBQUMsT0FBRDtPQUFLLFdBQVU7aUJBQW9DO01BQWtDOzs7OztNQUNyRix3QkFBQyxPQUFELEVBQUssV0FBVSxPQUFROzs7OztNQUN2Qix3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBMkI7TUFBMEI7Ozs7O0tBQ2pFOzs7O2VBQ0wsd0JBQUMsT0FBRDtNQUNFLHdCQUFDLE9BQUQ7T0FBSyxXQUFVO2lCQUFpQztNQUF5Qjs7Ozs7TUFDekUsd0JBQUMsT0FBRDtPQUFLLFdBQVU7aUJBQW9DO01BQW1DOzs7OztNQUN0Rix3QkFBQyxPQUFELEVBQUssV0FBVSxPQUFROzs7OztNQUN2Qix3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBMkI7TUFBeUI7Ozs7O0tBQ2hFOzs7O2FBQ0Y7Ozs7OztHQUNGOzs7OztVQUNGOzs7Ozs7QUFFVCIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJTdGF0aXN0aWNzVmlldy50c3giXSwidmVyc2lvbiI6Mywic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IFJlYWN0LCB7IHVzZVN0YXRlIH0gZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0IH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IEhBTk9JX0RJU1RSSUNUUyB9IGZyb20gJy4uL2RhdGEvaGFub2lEaXN0cmljdHMnO1xuaW1wb3J0IHtcbiAgQmFyQ2hhcnQzLFxuICBQcmludGVyLFxuICBEb3dubG9hZCxcbiAgU2hpZWxkLFxuICBBY3Rpdml0eSxcbiAgTGF5ZXJzLFxuICBGaWxlVGV4dCxcbiAgTWFwUGluLFxuICBDaGVja0NpcmNsZTIsXG59IGZyb20gJ2x1Y2lkZS1yZWFjdCc7XG5cbmludGVyZmFjZSBTdGF0aXN0aWNzVmlld1Byb3BzIHtcbiAgdW5pdHM6IE1pbGl0YXJ5VW5pdFtdO1xufVxuXG5leHBvcnQgY29uc3QgU3RhdGlzdGljc1ZpZXc6IFJlYWN0LkZDPFN0YXRpc3RpY3NWaWV3UHJvcHM+ID0gKHsgdW5pdHMgfSkgPT4ge1xuICBjb25zdCBbcHJpbnRNb2RlLCBzZXRQcmludE1vZGVdID0gdXNlU3RhdGUoZmFsc2UpO1xuXG4gIGNvbnN0IHRvdGFsID0gdW5pdHMubGVuZ3RoO1xuXG4gIC8vIEJyZWFrZG93biBieSBicmFuY2hcbiAgY29uc3QgYnJhbmNoQ291bnRzID0gdW5pdHMucmVkdWNlKChhY2MsIHUpID0+IHtcbiAgICBhY2NbdS5icmFuY2hdID0gKGFjY1t1LmJyYW5jaF0gfHwgMCkgKyAxO1xuICAgIHJldHVybiBhY2M7XG4gIH0sIHt9IGFzIFJlY29yZDxzdHJpbmcsIG51bWJlcj4pO1xuXG4gIC8vIEJyZWFrZG93biBieSByZWFkaW5lc3NcbiAgY29uc3QgcmVhZGluZXNzQ291bnRzID0gdW5pdHMucmVkdWNlKChhY2MsIHUpID0+IHtcbiAgICBhY2NbdS5yZWFkaW5lc3NTdGF0dXNdID0gKGFjY1t1LnJlYWRpbmVzc1N0YXR1c10gfHwgMCkgKyAxO1xuICAgIHJldHVybiBhY2M7XG4gIH0sIHt9IGFzIFJlY29yZDxzdHJpbmcsIG51bWJlcj4pO1xuXG4gIC8vIEJyZWFrZG93biBieSBzZWN1cml0eSBsZXZlbFxuICBjb25zdCBzZWN1cml0eUNvdW50cyA9IHVuaXRzLnJlZHVjZSgoYWNjLCB1KSA9PiB7XG4gICAgYWNjW3Uuc2VjdXJpdHlMZXZlbF0gPSAoYWNjW3Uuc2VjdXJpdHlMZXZlbF0gfHwgMCkgKyAxO1xuICAgIHJldHVybiBhY2M7XG4gIH0sIHt9IGFzIFJlY29yZDxzdHJpbmcsIG51bWJlcj4pO1xuXG4gIC8vIEJyZWFrZG93biBieSB1bml0IHR5cGVcbiAgY29uc3QgdHlwZUNvdW50cyA9IHVuaXRzLnJlZHVjZSgoYWNjLCB1KSA9PiB7XG4gICAgYWNjW3UudW5pdFR5cGVdID0gKGFjY1t1LnVuaXRUeXBlXSB8fCAwKSArIDE7XG4gICAgcmV0dXJuIGFjYztcbiAgfSwge30gYXMgUmVjb3JkPHN0cmluZywgbnVtYmVyPik7XG5cbiAgY29uc3QgaGFuZGxlUHJpbnQgPSAoKSA9PiB7XG4gICAgc2V0UHJpbnRNb2RlKHRydWUpO1xuICAgIHNldFRpbWVvdXQoKCkgPT4ge1xuICAgICAgd2luZG93LnByaW50KCk7XG4gICAgICBzZXRQcmludE1vZGUoZmFsc2UpO1xuICAgIH0sIDIwMCk7XG4gIH07XG5cbiAgcmV0dXJuIChcbiAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXgtMSBvdmVyZmxvdy15LWF1dG8gYmctWyMwODBkMTldIHRleHQtd2hpdGUgcC02IHNwYWNlLXktNlwiPlxuICAgICAgey8qIFRpdGxlICYgUHJpbnQgQWN0aW9uICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGZsZXgtd3JhcCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGdhcC00IGJvcmRlci1iIGJvcmRlci1zbGF0ZS04MDAgcGItNFwiPlxuICAgICAgICA8ZGl2PlxuICAgICAgICAgIDxoMiBjbGFzc05hbWU9XCJ0ZXh0LXhsIGZvbnQtYm9sZCB1cHBlcmNhc2UgdHJhY2tpbmctd2lkZXIgdGV4dC13aGl0ZSBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgPEJhckNoYXJ0MyBjbGFzc05hbWU9XCJ3LTUgaC01IHRleHQtZW1lcmFsZC00MDBcIiAvPlxuICAgICAgICAgICAgPHNwYW4+VGjhu5FuZyBrw6ogJiBM4bqtcCBCw6FvIGPDoW8gVOG7lW5nIGjhu6NwIEdJUzwvc3Bhbj5cbiAgICAgICAgICA8L2gyPlxuICAgICAgICAgIDxwIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlxuICAgICAgICAgICAgUGjDom4gdMOtY2ggc+G7kSBsaeG7h3UgdGjhu5FuZyBrw6ogbOG7sWMgbMaw4bujbmcsIGPGoSBj4bqldSBxdcOibiBiaW5oIGNo4bunbmcgdsOgIHh14bqldCB2xINuIGLhuqNuIGLDoW8gY8OhbyBr4bu5XG4gICAgICAgICAgICB0aHXhuq10XG4gICAgICAgICAgPC9wPlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGdhcC0zXCI+XG4gICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgb25DbGljaz17aGFuZGxlUHJpbnR9XG4gICAgICAgICAgICBjbGFzc05hbWU9XCJiZy1lbWVyYWxkLTYwMCBob3ZlcjpiZy1lbWVyYWxkLTUwMCB0ZXh0LXdoaXRlIHRleHQteHMgZm9udC1zZW1pYm9sZCBweC00IHB5LTIgcm91bmRlZC1sZyBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMiBzaGFkb3cgdHJhbnNpdGlvblwiXG4gICAgICAgICAgPlxuICAgICAgICAgICAgPFByaW50ZXIgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5JbiBCw6FvIEPDoW8gS+G7uSBUaHXhuq10IChQREYpPC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8L2Rpdj5cbiAgICAgIDwvZGl2PlxuXG4gICAgICB7LyogUHJpbnRhYmxlIFJlcG9ydCBTZWN0aW9uICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC14bCBwLTYgc2hhZG93LXhsIHNwYWNlLXktNlwiPlxuICAgICAgICB7LyogRm9ybWFsIFZpZXRuYW1lc2UgTWlsaXRhcnkgSGVhZGVyICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtY2VudGVyIHNwYWNlLXktMSBib3JkZXItYiBib3JkZXItc2xhdGUtNzAwLzgwIHBiLTRcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQteHMgdXBwZXJjYXNlIGZvbnQtc2VtaWJvbGQgdHJhY2tpbmctd2lkZXIgdGV4dC1zbGF0ZS0zMDBcIj5cbiAgICAgICAgICAgIEjhu4xDIFZJ4buGTiBL4bu4IFRIVeG6rFQgUVXDgk4gU+G7sCAtIELhu5ggUVXhu5BDIFBIw5JOR1xuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdXBwZXJjYXNlIGZvbnQtbWVkaXVtIHRleHQtc2xhdGUtNDAwXCI+XG4gICAgICAgICAgICBI4buGIFRI4buQTkcgQ8agIFPhu54gROG7riBMSeG7hlUgR0lTIFFV4bqiTiBMw50gxJDGoE4gVuG7iiBRVcOCTiDEkOG7mEkgSMOAIE7hu5hJXG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ3LTMyIGgtMC41IGJnLWVtZXJhbGQtNTAwIG14LWF1dG8gbXktMlwiIC8+XG4gICAgICAgICAgPGgxIGNsYXNzTmFtZT1cInRleHQtbGcgZm9udC1ib2xkIHRleHQtd2hpdGUgdXBwZXJjYXNlIHRyYWNraW5nLXdpZGVyXCI+XG4gICAgICAgICAgICBCw4FPIEPDgU8gVOG7lE5HIEjhu6JQIEThu64gTEnhu4ZVIEtIw5RORyBHSUFOIEdJUyBDw4FDIMSQxqBOIFbhu4ogVFLDik4gxJDhu4pBIELDgE4gVEjhu6YgxJDDlCBIw4AgTuG7mElcbiAgICAgICAgICA8L2gxPlxuICAgICAgICAgIDxwIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDBcIj5cbiAgICAgICAgICAgIChI4buHIHThu41hIMSR4buZIFZOLTIwMDAgLyBVVE0gWm9uZSA0OE4gLSBFUFNHOjMyNjQ4ICYgV0dTLTg0IC0gRVBTRzo0MzI2KVxuICAgICAgICAgIDwvcD5cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgey8qIDQgU3VtbWFyeSBTdGF0IEJveGVzICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTIgbWQ6Z3JpZC1jb2xzLTQgZ2FwLTRcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMDkwZjFkXSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCBwLTMgcm91bmRlZC1sZyB0ZXh0LWNlbnRlclwiPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9ja1wiPlxuICAgICAgICAgICAgICBU4buVbmcgc+G7kSDEkcahbiB24buLXG4gICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LTJ4bCBmb250LWJvbGQgdGV4dC1lbWVyYWxkLTQwMCBmb250LW1vbm9cIj57dG90YWx9PC9zcGFuPlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwOTBmMWRdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHAtMyByb3VuZGVkLWxnIHRleHQtY2VudGVyXCI+XG4gICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrXCI+XG4gICAgICAgICAgICAgIFNTQ8SQIEPhuqVwIDFcbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtMnhsIGZvbnQtYm9sZCB0ZXh0LXJvc2UtNDAwIGZvbnQtbW9ub1wiPlxuICAgICAgICAgICAgICB7cmVhZGluZXNzQ291bnRzWydD4bqlcCAxIChDYW8gbmjhuqV0KSddIHx8IDB9XG4gICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcC0zIHJvdW5kZWQtbGcgdGV4dC1jZW50ZXJcIj5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSBmb250LWJvbGQgYmxvY2tcIj5cbiAgICAgICAgICAgICAgU+G7kSBRdcOibiBCaW5oIENo4bunbmdcbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtMnhsIGZvbnQtYm9sZCB0ZXh0LXNreS00MDAgZm9udC1tb25vXCI+XG4gICAgICAgICAgICAgIHtPYmplY3Qua2V5cyhicmFuY2hDb3VudHMpLmxlbmd0aH1cbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMDkwZjFkXSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCBwLTMgcm91bmRlZC1sZyB0ZXh0LWNlbnRlclwiPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9ja1wiPlxuICAgICAgICAgICAgICDEkOG7mSBwaOG7pyBxdeG6rW4vaHV54buHblxuICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC0yeGwgZm9udC1ib2xkIHRleHQtYW1iZXItNDAwIGZvbnQtbW9ub1wiPjMwIC8gMzA8L3NwYW4+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHsvKiBBbmFseXRpY2FsIENoYXJ0cyAvIFByb2dyZXNzIEJhcnMgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMSBtZDpncmlkLWNvbHMtMiBnYXAtNiBwdC0yXCI+XG4gICAgICAgICAgey8qIDEuIE1pbGl0YXJ5IEJyYW5jaCBEaXN0cmlidXRpb24gKi99XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTMgYmctWyMwOTBmMWRdIHAtNCByb3VuZGVkLXhsIGJvcmRlciBib3JkZXItc2xhdGUtODAwXCI+XG4gICAgICAgICAgICA8aDMgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdXBwZXJjYXNlIHRyYWNraW5nLXdpZGVyIHRleHQtZW1lcmFsZC00MDAgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEuNVwiPlxuICAgICAgICAgICAgICA8TGF5ZXJzIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgICAgPHNwYW4+Q8ahIGPhuqV1IFF1w6JuIEJpbmggQ2jhu6duZzwvc3Bhbj5cbiAgICAgICAgICAgIDwvaDM+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInNwYWNlLXktMlwiPlxuICAgICAgICAgICAgICB7T2JqZWN0LmVudHJpZXMoYnJhbmNoQ291bnRzKS5tYXAoKFticmFuY2gsIGNvdW50XSkgPT4ge1xuICAgICAgICAgICAgICAgIGNvbnN0IHBjdCA9IE1hdGgucm91bmQoKGNvdW50IC8gdG90YWwpICogMTAwKTtcbiAgICAgICAgICAgICAgICByZXR1cm4gKFxuICAgICAgICAgICAgICAgICAgPGRpdiBrZXk9e2JyYW5jaH0+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gdGV4dC14cyBtYi0xXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS0zMDAgZm9udC1tZWRpdW1cIj57YnJhbmNofTwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBmb250LW1vbm9cIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIHtjb3VudH0gKHtwY3R9JSlcbiAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInctZnVsbCBoLTEuNSBiZy1zbGF0ZS04MDAgcm91bmRlZC1mdWxsIG92ZXJmbG93LWhpZGRlblwiPlxuICAgICAgICAgICAgICAgICAgICAgIDxkaXZcbiAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cImgtZnVsbCBiZy1lbWVyYWxkLTUwMCByb3VuZGVkLWZ1bGxcIlxuICAgICAgICAgICAgICAgICAgICAgICAgc3R5bGU9e3sgd2lkdGg6IGAke3BjdH0lYCB9fVxuICAgICAgICAgICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgICAgfSl9XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiAyLiBSZWFkaW5lc3MgU3RhdHVzICovfVxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS0zIGJnLVsjMDkwZjFkXSBwLTQgcm91bmRlZC14bCBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMFwiPlxuICAgICAgICAgICAgPGgzIGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1ib2xkIHVwcGVyY2FzZSB0cmFja2luZy13aWRlciB0ZXh0LWFtYmVyLTQwMCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICAgIDxBY3Rpdml0eSBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNVwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuPlRy4bqhbmcgdGjDoWkgU+G6tW4gc8OgbmcgQ2hp4bq/biDEkeG6pXUgKFNTQ8SQKTwvc3Bhbj5cbiAgICAgICAgICAgIDwvaDM+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInNwYWNlLXktMlwiPlxuICAgICAgICAgICAgICB7T2JqZWN0LmVudHJpZXMocmVhZGluZXNzQ291bnRzKS5tYXAoKFtzdGF0dXMsIGNvdW50XSkgPT4ge1xuICAgICAgICAgICAgICAgIGNvbnN0IHBjdCA9IE1hdGgucm91bmQoKGNvdW50IC8gdG90YWwpICogMTAwKTtcbiAgICAgICAgICAgICAgICBjb25zdCBjb2xvciA9IHN0YXR1cy5pbmNsdWRlcygnQ+G6pXAgMScpXG4gICAgICAgICAgICAgICAgICA/ICdiZy1yb3NlLTUwMCdcbiAgICAgICAgICAgICAgICAgIDogc3RhdHVzLmluY2x1ZGVzKCdD4bqlcCAyJylcbiAgICAgICAgICAgICAgICAgID8gJ2JnLWFtYmVyLTUwMCdcbiAgICAgICAgICAgICAgICAgIDogJ2JnLWVtZXJhbGQtNTAwJztcblxuICAgICAgICAgICAgICAgIHJldHVybiAoXG4gICAgICAgICAgICAgICAgICA8ZGl2IGtleT17c3RhdHVzfT5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGp1c3RpZnktYmV0d2VlbiB0ZXh0LXhzIG1iLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTMwMCBmb250LW1lZGl1bVwiPntzdGF0dXN9PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwIGZvbnQtbW9ub1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAge2NvdW50fSAoe3BjdH0lKVxuICAgICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidy1mdWxsIGgtMS41IGJnLXNsYXRlLTgwMCByb3VuZGVkLWZ1bGwgb3ZlcmZsb3ctaGlkZGVuXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPGRpdlxuICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPXtgaC1mdWxsIHJvdW5kZWQtZnVsbCAke2NvbG9yfWB9XG4gICAgICAgICAgICAgICAgICAgICAgICBzdHlsZT17eyB3aWR0aDogYCR7cGN0fSVgIH19XG4gICAgICAgICAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgICB9KX1cbiAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICB7LyogU2VjdXJpdHkgTGV2ZWwgQnJlYWtkb3duICovfVxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJwdC00IG10LTQgYm9yZGVyLXQgYm9yZGVyLXNsYXRlLTgwMCBzcGFjZS15LTJcIj5cbiAgICAgICAgICAgICAgPGg0IGNsYXNzTmFtZT1cInRleHQtWzExcHhdIGZvbnQtYm9sZCB1cHBlcmNhc2UgdGV4dC1zbGF0ZS00MDBcIj5cbiAgICAgICAgICAgICAgICBQaMOibiBsb+G6oWkgY+G6pXAgxJHhu5kgYuG6o28gbeG6rXQgQ1NETDpcbiAgICAgICAgICAgICAgPC9oND5cbiAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0yIGdhcC0yIHRleHQteHNcIj5cbiAgICAgICAgICAgICAgICB7T2JqZWN0LmVudHJpZXMoc2VjdXJpdHlDb3VudHMpLm1hcCgoW3NlYywgY291bnRdKSA9PiAoXG4gICAgICAgICAgICAgICAgICA8ZGl2XG4gICAgICAgICAgICAgICAgICAgIGtleT17c2VjfVxuICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJwLTIgcm91bmRlZCBiZy1zbGF0ZS04MDAvNjAgYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAvNjAgZmxleCBqdXN0aWZ5LWJldHdlZW5cIlxuICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTMwMFwiPntzZWN9Ojwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPHN0cm9uZyBjbGFzc05hbWU9XCJ0ZXh0LWVtZXJhbGQtNDAwIGZvbnQtbW9ub1wiPntjb3VudH08L3N0cm9uZz5cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogVW5pdHMgRGlzdHJpYnV0aW9uIFRhYmxlICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInNwYWNlLXktM1wiPlxuICAgICAgICAgIDxoMyBjbGFzc05hbWU9XCJ0ZXh0LXhzIGZvbnQtYm9sZCB1cHBlcmNhc2UgdHJhY2tpbmctd2lkZXIgdGV4dC13aGl0ZSBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICA8RmlsZVRleHQgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjUgdGV4dC1za3ktNDAwXCIgLz5cbiAgICAgICAgICAgIDxzcGFuPkRhbmggbeG7pWMgVOG7lW5nIGjhu6NwIGPDoWMgxJDGoW4gduG7iyBRdcOibiBz4buxIMSRw6MgU+G7kSBow7NhPC9zcGFuPlxuICAgICAgICAgIDwvaDM+XG5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cIm92ZXJmbG93LXgtYXV0byBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLWxnXCI+XG4gICAgICAgICAgICA8dGFibGUgY2xhc3NOYW1lPVwidy1mdWxsIHRleHQtbGVmdCB0ZXh0LXhzIHRleHQtc2xhdGUtMzAwXCI+XG4gICAgICAgICAgICAgIDx0aGVhZCBjbGFzc05hbWU9XCJiZy1bIzA5MGYxZF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIHRleHQtWzEwcHhdIGZvbnQtbW9ubyBib3JkZXItYiBib3JkZXItc2xhdGUtODAwXCI+XG4gICAgICAgICAgICAgICAgPHRyPlxuICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtMi41XCI+VFQ8L3RoPlxuICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtMi41XCI+TcOjIFFTPC90aD5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTIuNVwiPlTDqm4gxJDGoW4gduG7izwvdGg+XG4gICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0yLjVcIj5CaW5oIGNo4bunbmc8L3RoPlxuICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtMi41XCI+UXXhuq1uIC8gSHV54buHbjwvdGg+XG4gICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0yLjVcIj5YIChWTi0yMDAwIG0pPC90aD5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTIuNVwiPlkgKFZOLTIwMDAgbSk8L3RoPlxuICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtMi41XCI+VHLhu7FjIGJhbjwvdGg+XG4gICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0yLjVcIj5UcuG6oW5nIHRow6FpIFNTQ8SQPC90aD5cbiAgICAgICAgICAgICAgICA8L3RyPlxuICAgICAgICAgICAgICA8L3RoZWFkPlxuICAgICAgICAgICAgICA8dGJvZHkgY2xhc3NOYW1lPVwiZGl2aWRlLXkgZGl2aWRlLXNsYXRlLTgwMC84MCBmb250LXNhbnNcIj5cbiAgICAgICAgICAgICAgICB7dW5pdHMubWFwKCh1LCBpZHgpID0+IChcbiAgICAgICAgICAgICAgICAgIDx0ciBrZXk9e3UuaWR9IGNsYXNzTmFtZT1cImhvdmVyOmJnLXNsYXRlLTgwMC80MFwiPlxuICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0yLjUgZm9udC1tb25vIHRleHQtc2xhdGUtNDAwXCI+e2lkeCArIDF9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMi41IGZvbnQtbW9ubyBmb250LWJvbGQgdGV4dC1lbWVyYWxkLTQwMFwiPnt1LmNvZGV9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMi41IGZvbnQtc2VtaWJvbGQgdGV4dC13aGl0ZVwiPnt1Lm5hbWV9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMi41XCI+e3UuYnJhbmNofTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTIuNVwiPnt1LmRpc3RyaWN0fTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTIuNSBmb250LW1vbm8gdGV4dC1za3ktMzAwXCI+e3UueF92bjIwMDAudG9Mb2NhbGVTdHJpbmcoKX08L3RkPlxuICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0yLjUgZm9udC1tb25vIHRleHQtc2t5LTMwMFwiPnt1Lnlfdm4yMDAwLnRvTG9jYWxlU3RyaW5nKCl9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMi41IGZvbnQtbW9ubyB0ZXh0LWVtZXJhbGQtMzAwXCI+e3UucGhvbmV9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMi41XCI+e3UucmVhZGluZXNzU3RhdHVzfTwvdGQ+XG4gICAgICAgICAgICAgICAgICA8L3RyPlxuICAgICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgICA8L3Rib2R5PlxuICAgICAgICAgICAgPC90YWJsZT5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgey8qIEZvcm1hbCBTaWduYXR1cmUgQm94ICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInB0LTggYm9yZGVyLXQgYm9yZGVyLXNsYXRlLTgwMCBncmlkIGdyaWQtY29scy0yIHRleHQtY2VudGVyIHRleHQteHMgdGV4dC1zbGF0ZS0zMDBcIj5cbiAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmb250LWJvbGQgdXBwZXJjYXNlIHRleHQtd2hpdGVcIj5OR8av4bucSSBM4bqsUCBCw4FPIEPDgU88L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMC41XCI+S+G7uSBzxrAgVHLhuq9jIMSR4buLYSAtIELhuqNuIMSR4buTICYgR0lTPC9kaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImgtMTZcIiAvPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmb250LXNlbWlib2xkIHRleHQtd2hpdGVcIj4oS8O9IHbDoCBnaGkgcsO1IGjhu40gdMOqbik8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmb250LWJvbGQgdXBwZXJjYXNlIHRleHQtd2hpdGVcIj5USOG7piBUUsav4bueTkcgUEjDiiBEVVnhu4ZUPC9kaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtWzExcHhdIHRleHQtc2xhdGUtNDAwIG10LTAuNVwiPlPhu58gQ2jhu4kgaHV5IC8gQ8ahIHF1YW4gVMOhYyBjaGnhur9uPC9kaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImgtMTZcIiAvPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmb250LXNlbWlib2xkIHRleHQtd2hpdGVcIj4oS8O9IHTDqm4gdsOgIMSRw7NuZyBk4bqldSk8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG4gICAgICA8L2Rpdj5cbiAgICA8L2Rpdj5cbiAgKTtcbn07XG4iXX0=