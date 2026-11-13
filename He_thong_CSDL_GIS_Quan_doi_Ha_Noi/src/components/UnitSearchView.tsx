const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"]; const useMemo = __vite__cjsImport0_react["useMemo"];const _jsxDEV = __vite__cjsImport3_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { HANOI_DISTRICTS } from "/src/data/hanoiDistricts.ts";
import { Search, Filter, MapPin, Compass, Phone, Shield, LayoutGrid, List, RotateCcw, CircleDot } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/UnitSearchView.tsx";
import __vite__cjsImport3_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const UnitSearchView = ({ units, onSelectUnitAndGoToMap, onTriggerBuffer }) => {
	const [searchTerm, setSearchTerm] = useState("");
	const [selectedBranch, setSelectedBranch] = useState("");
	const [selectedType, setSelectedType] = useState("");
	const [selectedDistrict, setSelectedDistrict] = useState("");
	const [selectedReadiness, setSelectedReadiness] = useState("");
	const [selectedSecurity, setSelectedSecurity] = useState("");
	const [viewMode, setViewMode] = useState("grid");
	// Unique options
	const branches = useMemo(() => Array.from(new Set(units.map((u) => u.branch))).sort(), [units]);
	const unitTypes = useMemo(() => Array.from(new Set(units.map((u) => u.unitType))).sort(), [units]);
	// Filtered units
	const filteredUnits = useMemo(() => {
		return units.filter((u) => {
			const matchSearch = searchTerm === "" || u.name.toLowerCase().includes(searchTerm.toLowerCase()) || u.code.toLowerCase().includes(searchTerm.toLowerCase()) || u.shortName.toLowerCase().includes(searchTerm.toLowerCase()) || u.address.toLowerCase().includes(searchTerm.toLowerCase()) || u.commander.toLowerCase().includes(searchTerm.toLowerCase());
			const matchBranch = selectedBranch === "" || u.branch === selectedBranch;
			const matchType = selectedType === "" || u.unitType === selectedType;
			const matchDistrict = selectedDistrict === "" || u.district === selectedDistrict;
			const matchReadiness = selectedReadiness === "" || u.readinessStatus.includes(selectedReadiness);
			const matchSecurity = selectedSecurity === "" || u.securityLevel === selectedSecurity;
			return matchSearch && matchBranch && matchType && matchDistrict && matchReadiness && matchSecurity;
		});
	}, [
		units,
		searchTerm,
		selectedBranch,
		selectedType,
		selectedDistrict,
		selectedReadiness,
		selectedSecurity
	]);
	const handleResetFilters = () => {
		setSearchTerm("");
		setSelectedBranch("");
		setSelectedType("");
		setSelectedDistrict("");
		setSelectedReadiness("");
		setSelectedSecurity("");
	};
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex-1 overflow-y-auto bg-[#080d19] text-white p-6 space-y-6",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex flex-wrap items-center justify-between gap-4 border-b border-slate-800 pb-4",
				children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h2", {
					className: "text-xl font-bold uppercase tracking-wider text-white flex items-center gap-2",
					children: [/* @__PURE__ */ _jsxDEV(Search, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 89,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tra cứu Thông tin Đơn vị Quân sự" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 90,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 88,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("p", {
					className: "text-xs text-slate-400 mt-1",
					children: "Bộ lọc đa tiêu chí theo không gian hành chính, quân binh chủng và trạng thái sẵn sàng chiến đấu"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 92,
					columnNumber: 11
				}, this)] }, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 87,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "flex items-center bg-slate-900 border border-slate-700 rounded-lg p-1",
					children: [/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => setViewMode("grid"),
						className: `p-1.5 rounded text-xs flex items-center gap-1 transition ${viewMode === "grid" ? "bg-emerald-600 text-white font-semibold" : "text-slate-400 hover:text-white"}`,
						children: [/* @__PURE__ */ _jsxDEV(LayoutGrid, { className: "w-3.5 h-3.5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 108,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Dạng Thẻ" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 109,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 100,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("button", {
						onClick: () => setViewMode("table"),
						className: `p-1.5 rounded text-xs flex items-center gap-1 transition ${viewMode === "table" ? "bg-emerald-600 text-white font-semibold" : "text-slate-400 hover:text-white"}`,
						children: [/* @__PURE__ */ _jsxDEV(List, { className: "w-3.5 h-3.5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 119,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Dạng Bảng GIS" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 120,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 111,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 99,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 86,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md space-y-3",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-xs font-bold uppercase text-emerald-400 flex items-center gap-1.5",
							children: [/* @__PURE__ */ _jsxDEV(Filter, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 129,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "BỘ LỌC TÌM KIẾM ĐA TIÊU CHÍ" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 130,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 128,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("button", {
							onClick: handleResetFilters,
							className: "text-xs text-slate-400 hover:text-rose-400 flex items-center gap-1 transition",
							children: [/* @__PURE__ */ _jsxDEV(RotateCcw, { className: "w-3 h-3" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 136,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Đặt lại bộ lọc" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 137,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 132,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 127,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-6 gap-3",
						children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "lg:col-span-2",
								children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
									children: "Từ khóa tìm kiếm"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 144,
									columnNumber: 13
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "relative",
									children: [/* @__PURE__ */ _jsxDEV("input", {
										type: "text",
										placeholder: "Tên, mã số, địa chỉ, chỉ huy...",
										value: searchTerm,
										onChange: (e) => setSearchTerm(e.target.value),
										className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-3 py-2 text-xs text-white placeholder-slate-500 focus:outline-none focus:border-emerald-500 pl-8"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 148,
										columnNumber: 15
									}, this), /* @__PURE__ */ _jsxDEV(Search, { className: "w-3.5 h-3.5 text-slate-400 absolute left-2.5 top-3" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 155,
										columnNumber: 15
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 147,
									columnNumber: 13
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 143,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Quân Binh chủng"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 161,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: selectedBranch,
								onChange: (e) => setSelectedBranch(e.target.value),
								className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-2.5 py-2 text-xs text-white focus:outline-none focus:border-emerald-500",
								children: [/* @__PURE__ */ _jsxDEV("option", {
									value: "",
									children: "Tất cả Binh chủng"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 169,
									columnNumber: 15
								}, this), branches.map((b) => /* @__PURE__ */ _jsxDEV("option", {
									value: b,
									children: b
								}, b, false, {
									fileName: _jsxFileName,
									lineNumber: 171,
									columnNumber: 17
								}, this))]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 164,
								columnNumber: 13
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 160,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Địa bàn Quận / Huyện"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 180,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: selectedDistrict,
								onChange: (e) => setSelectedDistrict(e.target.value),
								className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-2.5 py-2 text-xs text-white focus:outline-none focus:border-emerald-500",
								children: [/* @__PURE__ */ _jsxDEV("option", {
									value: "",
									children: "Tất cả 30 Quận/Huyện"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 188,
									columnNumber: 15
								}, this), HANOI_DISTRICTS.map((d) => /* @__PURE__ */ _jsxDEV("option", {
									value: d.name,
									children: d.name
								}, d.code, false, {
									fileName: _jsxFileName,
									lineNumber: 190,
									columnNumber: 17
								}, this))]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 183,
								columnNumber: 13
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 179,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Trạng thái SSCĐ"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 199,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: selectedReadiness,
								onChange: (e) => setSelectedReadiness(e.target.value),
								className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-2.5 py-2 text-xs text-white focus:outline-none focus:border-emerald-500",
								children: [
									/* @__PURE__ */ _jsxDEV("option", {
										value: "",
										children: "Tất cả trạng thái"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 207,
										columnNumber: 15
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Cấp 1",
										children: "Cấp 1 (Cao nhất)"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 208,
										columnNumber: 15
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Cấp 2",
										children: "Cấp 2 (Tăng cường)"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 209,
										columnNumber: 15
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Cấp 3",
										children: "Cấp 3 (Thường xuyên)"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 210,
										columnNumber: 15
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Huấn luyện",
										children: "Huấn luyện - Đào tạo"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 211,
										columnNumber: 15
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 202,
								columnNumber: 13
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 198,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Cấp / Loại hình"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 217,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: selectedType,
								onChange: (e) => setSelectedType(e.target.value),
								className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-2.5 py-2 text-xs text-white focus:outline-none focus:border-emerald-500",
								children: [/* @__PURE__ */ _jsxDEV("option", {
									value: "",
									children: "Tất cả các cấp"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 225,
									columnNumber: 15
								}, this), unitTypes.map((t) => /* @__PURE__ */ _jsxDEV("option", {
									value: t,
									children: t
								}, t, false, {
									fileName: _jsxFileName,
									lineNumber: 227,
									columnNumber: 17
								}, this))]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 220,
								columnNumber: 13
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 216,
								columnNumber: 11
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 141,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex justify-between items-center text-xs text-slate-400 pt-2 border-t border-slate-800",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [
							"Tìm thấy ",
							/* @__PURE__ */ _jsxDEV("strong", {
								className: "text-emerald-400 font-mono",
								children: filteredUnits.length
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 238,
								columnNumber: 22
							}, this),
							" ",
							"kết quả phù hợp"
						] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 237,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "text-[11px] text-slate-400",
							children: ["Hệ quy chiếu: ", /* @__PURE__ */ _jsxDEV("span", {
								className: "font-mono text-sky-400",
								children: "VN-2000 / EPSG:32648"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 242,
								columnNumber: 27
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 241,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 236,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 126,
				columnNumber: 7
			}, this),
			viewMode === "grid" ? /* @__PURE__ */ _jsxDEV("div", {
				className: "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4",
				children: filteredUnits.map((unit) => /* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 hover:border-emerald-600 rounded-xl p-4 shadow-md transition-all flex flex-col justify-between",
					children: [/* @__PURE__ */ _jsxDEV("div", { children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center justify-between gap-2 mb-2",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "font-mono text-xs px-2 py-0.5 rounded bg-emerald-950 text-emerald-300 border border-emerald-800 font-bold",
								children: unit.code
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 258,
								columnNumber: 19
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: `text-[10px] px-2 py-0.5 rounded font-medium ${unit.readinessStatus.includes("Cấp 1") ? "bg-rose-950 text-rose-300 border border-rose-800" : "bg-emerald-950 text-emerald-300 border border-emerald-800"}`,
								children: unit.readinessStatus
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 261,
								columnNumber: 19
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 257,
							columnNumber: 17
						}, this),
						/* @__PURE__ */ _jsxDEV("h3", {
							className: "text-sm font-bold text-white line-clamp-2 leading-snug",
							children: unit.name
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 272,
							columnNumber: 17
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "text-[11px] text-slate-400 mt-1 font-medium",
							children: unit.parentUnit
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 275,
							columnNumber: 17
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "mt-3 space-y-1.5 text-xs text-slate-300",
							children: [
								/* @__PURE__ */ _jsxDEV("div", {
									className: "flex items-center gap-1.5",
									children: [/* @__PURE__ */ _jsxDEV(Shield, { className: "w-3.5 h-3.5 text-amber-400 flex-shrink-0" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 280,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("span", { children: ["Binh chủng: ", /* @__PURE__ */ _jsxDEV("strong", { children: unit.branch }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 282,
										columnNumber: 35
									}, this)] }, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 281,
										columnNumber: 21
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 279,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("div", {
									className: "flex items-start gap-1.5",
									children: [/* @__PURE__ */ _jsxDEV(MapPin, { className: "w-3.5 h-3.5 text-sky-400 flex-shrink-0 mt-0.5" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 286,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("span", {
										className: "line-clamp-1",
										children: unit.address
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 287,
										columnNumber: 21
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 285,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("div", {
									className: "flex items-center gap-1.5",
									children: [/* @__PURE__ */ _jsxDEV(Phone, { className: "w-3.5 h-3.5 text-emerald-400 flex-shrink-0" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 290,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("span", {
										className: "font-mono text-slate-300",
										children: unit.phone
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 291,
										columnNumber: 21
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 289,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("div", {
									className: "p-2 bg-[#090f1d] rounded border border-slate-800 text-[10px] font-mono text-sky-300",
									children: [
										"X: ",
										unit.x_vn2000.toLocaleString(),
										"m | Y: ",
										unit.y_vn2000.toLocaleString(),
										"m"
									]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 293,
									columnNumber: 19
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 278,
							columnNumber: 17
						}, this)
					] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 255,
						columnNumber: 15
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "mt-4 pt-3 border-t border-slate-800 flex gap-2",
						children: [/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => onSelectUnitAndGoToMap(unit),
							className: "flex-1 bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold py-2 px-3 rounded flex items-center justify-center gap-1.5 shadow transition",
							children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 305,
								columnNumber: 19
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Xem trên bản đồ" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 306,
								columnNumber: 19
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 301,
							columnNumber: 17
						}, this), /* @__PURE__ */ _jsxDEV("button", {
							onClick: () => onTriggerBuffer(unit),
							title: "Tạo vùng đệm phòng thủ bán kính 10km",
							className: "bg-slate-800 hover:bg-amber-700 text-slate-300 hover:text-white text-xs p-2 rounded transition",
							children: /* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 313,
								columnNumber: 19
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 308,
							columnNumber: 17
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 300,
						columnNumber: 15
					}, this)]
				}, unit.id, true, {
					fileName: _jsxFileName,
					lineNumber: 251,
					columnNumber: 13
				}, this))
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 249,
				columnNumber: 9
			}, this) : 
			/* Table View */
			/* @__PURE__ */ _jsxDEV("div", {
				className: "bg-[#0e1628] border border-slate-800 rounded-xl overflow-hidden shadow-lg",
				children: /* @__PURE__ */ _jsxDEV("div", {
					className: "overflow-x-auto",
					children: /* @__PURE__ */ _jsxDEV("table", {
						className: "w-full text-left text-xs text-slate-300",
						children: [/* @__PURE__ */ _jsxDEV("thead", {
							className: "bg-[#0b1220] text-slate-400 uppercase text-[10px] tracking-wider font-mono border-b border-slate-800",
							children: /* @__PURE__ */ _jsxDEV("tr", { children: [
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Mã đơn vị"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 326,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Tên đơn vị"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 327,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Binh chủng"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 328,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Quận / Huyện"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 329,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Tọa độ VN-2000 (m)"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 330,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Chỉ huy trưởng"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 331,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Trực ban"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 332,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "SSCĐ"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 333,
									columnNumber: 19
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3 text-right",
									children: "Thao tác"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 334,
									columnNumber: 19
								}, this)
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 325,
								columnNumber: 17
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 324,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("tbody", {
							className: "divide-y divide-slate-800",
							children: filteredUnits.map((u) => /* @__PURE__ */ _jsxDEV("tr", {
								className: "hover:bg-slate-800/50 transition",
								children: [
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-mono font-bold text-emerald-400",
										children: u.code
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 340,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3",
										children: [/* @__PURE__ */ _jsxDEV("div", {
											className: "font-bold text-white",
											children: u.name
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 342,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("div", {
											className: "text-[10px] text-slate-400",
											children: u.parentUnit
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 343,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 341,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3",
										children: u.branch
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 345,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3",
										children: u.district
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 346,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-mono text-[11px] text-sky-300",
										children: [
											"X: ",
											u.x_vn2000.toLocaleString(),
											/* @__PURE__ */ _jsxDEV("br", {}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 349,
												columnNumber: 23
											}, this),
											"Y: ",
											u.y_vn2000.toLocaleString()
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 347,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3",
										children: u.commander || "-"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 352,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-mono text-emerald-300",
										children: u.phone
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 353,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3",
										children: /* @__PURE__ */ _jsxDEV("span", {
											className: `text-[10px] px-2 py-0.5 rounded font-medium ${u.readinessStatus.includes("Cấp 1") ? "bg-rose-950 text-rose-300" : "bg-emerald-950 text-emerald-300"}`,
											children: u.readinessStatus
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 355,
											columnNumber: 23
										}, this)
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 354,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 text-right",
										children: /* @__PURE__ */ _jsxDEV("button", {
											onClick: () => onSelectUnitAndGoToMap(u),
											className: "bg-emerald-700/80 hover:bg-emerald-600 text-white px-2.5 py-1 rounded text-[11px] font-semibold",
											children: "Xem GIS"
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 366,
											columnNumber: 23
										}, this)
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 365,
										columnNumber: 21
									}, this)
								]
							}, u.id, true, {
								fileName: _jsxFileName,
								lineNumber: 339,
								columnNumber: 19
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 337,
							columnNumber: 15
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 323,
						columnNumber: 13
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 322,
					columnNumber: 11
				}, this)
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 321,
				columnNumber: 9
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 84,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,eyJtYXBwaW5ncyI6IkFBQUEsT0FBTyxTQUFTLFVBQVUsZUFBZTtBQUV6QyxTQUFTLHVCQUF1QjtBQUNoQyxTQUNFLFFBQ0EsUUFDQSxRQUNBLFNBQ0EsT0FDQSxRQUdBLFlBQ0EsTUFDQSxXQUNBLGlCQUVLOzs7QUFRUCxPQUFPLE1BQU0sa0JBQWlELEVBQzVELE9BQ0Esd0JBQ0Esc0JBQ0k7Q0FDSixNQUFNLENBQUMsWUFBWSxpQkFBaUIsU0FBUyxFQUFFO0NBQy9DLE1BQU0sQ0FBQyxnQkFBZ0IscUJBQXFCLFNBQVMsRUFBRTtDQUN2RCxNQUFNLENBQUMsY0FBYyxtQkFBbUIsU0FBUyxFQUFFO0NBQ25ELE1BQU0sQ0FBQyxrQkFBa0IsdUJBQXVCLFNBQVMsRUFBRTtDQUMzRCxNQUFNLENBQUMsbUJBQW1CLHdCQUF3QixTQUFTLEVBQUU7Q0FDN0QsTUFBTSxDQUFDLGtCQUFrQix1QkFBdUIsU0FBUyxFQUFFO0NBQzNELE1BQU0sQ0FBQyxVQUFVLGVBQWUsU0FBMkIsTUFBTTs7Q0FHakUsTUFBTSxXQUFXLGNBQWMsTUFBTSxLQUFLLElBQUksSUFBSSxNQUFNLEtBQUssTUFBTSxFQUFFLE1BQU0sQ0FBQyxDQUFDLENBQUMsQ0FBQyxLQUFLLEdBQUcsQ0FBQyxLQUFLLENBQUM7Q0FDOUYsTUFBTSxZQUFZLGNBQWMsTUFBTSxLQUFLLElBQUksSUFBSSxNQUFNLEtBQUssTUFBTSxFQUFFLFFBQVEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxLQUFLLEdBQUcsQ0FBQyxLQUFLLENBQUM7O0NBR2pHLE1BQU0sZ0JBQWdCLGNBQWM7RUFDbEMsT0FBTyxNQUFNLFFBQVEsTUFBTTtHQUN6QixNQUFNLGNBQ0osZUFBZSxNQUNmLEVBQUUsS0FBSyxZQUFZLENBQUMsQ0FBQyxTQUFTLFdBQVcsWUFBWSxDQUFDLEtBQ3RELEVBQUUsS0FBSyxZQUFZLENBQUMsQ0FBQyxTQUFTLFdBQVcsWUFBWSxDQUFDLEtBQ3RELEVBQUUsVUFBVSxZQUFZLENBQUMsQ0FBQyxTQUFTLFdBQVcsWUFBWSxDQUFDLEtBQzNELEVBQUUsUUFBUSxZQUFZLENBQUMsQ0FBQyxTQUFTLFdBQVcsWUFBWSxDQUFDLEtBQ3pELEVBQUUsVUFBVSxZQUFZLENBQUMsQ0FBQyxTQUFTLFdBQVcsWUFBWSxDQUFDO0dBRTdELE1BQU0sY0FBYyxtQkFBbUIsTUFBTSxFQUFFLFdBQVc7R0FDMUQsTUFBTSxZQUFZLGlCQUFpQixNQUFNLEVBQUUsYUFBYTtHQUN4RCxNQUFNLGdCQUFnQixxQkFBcUIsTUFBTSxFQUFFLGFBQWE7R0FDaEUsTUFBTSxpQkFBaUIsc0JBQXNCLE1BQU0sRUFBRSxnQkFBZ0IsU0FBUyxpQkFBaUI7R0FDL0YsTUFBTSxnQkFBZ0IscUJBQXFCLE1BQU0sRUFBRSxrQkFBa0I7R0FFckUsT0FDRSxlQUFlLGVBQWUsYUFBYSxpQkFBaUIsa0JBQWtCO0VBRWxGLENBQUM7Q0FDSCxHQUFHO0VBQ0Q7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7Q0FDRixDQUFDO0NBRUQsTUFBTSwyQkFBMkI7RUFDL0IsY0FBYyxFQUFFO0VBQ2hCLGtCQUFrQixFQUFFO0VBQ3BCLGdCQUFnQixFQUFFO0VBQ2xCLG9CQUFvQixFQUFFO0VBQ3RCLHFCQUFxQixFQUFFO0VBQ3ZCLG9CQUFvQixFQUFFO0NBQ3hCO0NBRUEsT0FDRSx3QkFBQyxPQUFEO0VBQUssV0FBVTtZQUFmO0dBRUUsd0JBQUMsT0FBRDtJQUFLLFdBQVU7Y0FBZixDQUNFLHdCQUFDLE9BQUQsYUFDRSx3QkFBQyxNQUFEO0tBQUksV0FBVTtlQUFkLENBQ0Usd0JBQUMsUUFBRCxFQUFRLFdBQVUsMkJBQTRCOzs7O2VBQzlDLHdCQUFDLFFBQUQsWUFBTSxtQ0FBc0M7Ozs7YUFDMUM7Ozs7O2NBQ0osd0JBQUMsS0FBRDtLQUFHLFdBQVU7ZUFBOEI7SUFHeEM7Ozs7WUFDQTs7OztjQUdMLHdCQUFDLE9BQUQ7S0FBSyxXQUFVO2VBQWYsQ0FDRSx3QkFBQyxVQUFEO01BQ0UsZUFBZSxZQUFZLE1BQU07TUFDakMsV0FBVyw0REFDVCxhQUFhLFNBQ1QsNENBQ0E7Z0JBTFIsQ0FRRSx3QkFBQyxZQUFELEVBQVksV0FBVSxjQUFlOzs7O2dCQUNyQyx3QkFBQyxRQUFELFlBQU0sV0FBYzs7OztjQUNkOzs7OztlQUNSLHdCQUFDLFVBQUQ7TUFDRSxlQUFlLFlBQVksT0FBTztNQUNsQyxXQUFXLDREQUNULGFBQWEsVUFDVCw0Q0FDQTtnQkFMUixDQVFFLHdCQUFDLE1BQUQsRUFBTSxXQUFVLGNBQWU7Ozs7Z0JBQy9CLHdCQUFDLFFBQUQsWUFBTSxnQkFBbUI7Ozs7Y0FDbkI7Ozs7O2FBQ0w7Ozs7O1lBQ0Y7Ozs7OztHQUdMLHdCQUFDLE9BQUQ7SUFBSyxXQUFVO2NBQWY7S0FDRSx3QkFBQyxPQUFEO01BQUssV0FBVTtnQkFBZixDQUNFLHdCQUFDLFFBQUQ7T0FBTSxXQUFVO2lCQUFoQixDQUNFLHdCQUFDLFFBQUQsRUFBUSxXQUFVLGNBQWU7Ozs7aUJBQ2pDLHdCQUFDLFFBQUQsWUFBTSw4QkFBaUM7Ozs7ZUFDbkM7Ozs7O2dCQUNOLHdCQUFDLFVBQUQ7T0FDRSxTQUFTO09BQ1QsV0FBVTtpQkFGWixDQUlFLHdCQUFDLFdBQUQsRUFBVyxXQUFVLFVBQVc7Ozs7aUJBQ2hDLHdCQUFDLFFBQUQsWUFBTSxpQkFBb0I7Ozs7ZUFDcEI7Ozs7O2NBQ0w7Ozs7OztLQUVMLHdCQUFDLE9BQUQ7TUFBSyxXQUFVO2dCQUFmO09BRUUsd0JBQUMsT0FBRDtRQUFLLFdBQVU7a0JBQWYsQ0FDRSx3QkFBQyxTQUFEO1NBQU8sV0FBVTttQkFBNEQ7UUFFdEU7Ozs7a0JBQ1Asd0JBQUMsT0FBRDtTQUFLLFdBQVU7bUJBQWYsQ0FDRSx3QkFBQyxTQUFEO1VBQ0UsTUFBSztVQUNMLGFBQVk7VUFDWixPQUFPO1VBQ1AsV0FBVyxNQUFNLGNBQWMsRUFBRSxPQUFPLEtBQUs7VUFDN0MsV0FBVTtTQUNYOzs7O21CQUNELHdCQUFDLFFBQUQsRUFBUSxXQUFVLHFEQUFzRDs7OztpQkFDckU7Ozs7O2dCQUNGOzs7Ozs7T0FHTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtRQUFPLFdBQVU7a0JBQTREO09BRXRFOzs7O2lCQUNQLHdCQUFDLFVBQUQ7UUFDRSxPQUFPO1FBQ1AsV0FBVyxNQUFNLGtCQUFrQixFQUFFLE9BQU8sS0FBSztRQUNqRCxXQUFVO2tCQUhaLENBS0Usd0JBQUMsVUFBRDtTQUFRLE9BQU07bUJBQUc7UUFBeUI7Ozs7a0JBQ3pDLFNBQVMsS0FBSyxNQUNiLHdCQUFDLFVBQUQ7U0FBZ0IsT0FBTzttQkFDcEI7UUFDSyxHQUZLOzs7O2VBRUwsQ0FDVCxDQUNLOzs7OztlQUNMOzs7OztPQUdMLHdCQUFDLE9BQUQsYUFDRSx3QkFBQyxTQUFEO1FBQU8sV0FBVTtrQkFBNEQ7T0FFdEU7Ozs7aUJBQ1Asd0JBQUMsVUFBRDtRQUNFLE9BQU87UUFDUCxXQUFXLE1BQU0sb0JBQW9CLEVBQUUsT0FBTyxLQUFLO1FBQ25ELFdBQVU7a0JBSFosQ0FLRSx3QkFBQyxVQUFEO1NBQVEsT0FBTTttQkFBRztRQUE0Qjs7OztrQkFDNUMsZ0JBQWdCLEtBQUssTUFDcEIsd0JBQUMsVUFBRDtTQUFxQixPQUFPLEVBQUU7bUJBQzNCLEVBQUU7UUFDRyxHQUZLLEVBQUU7Ozs7ZUFFUCxDQUNULENBQ0s7Ozs7O2VBQ0w7Ozs7O09BR0wsd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFNBQUQ7UUFBTyxXQUFVO2tCQUE0RDtPQUV0RTs7OztpQkFDUCx3QkFBQyxVQUFEO1FBQ0UsT0FBTztRQUNQLFdBQVcsTUFBTSxxQkFBcUIsRUFBRSxPQUFPLEtBQUs7UUFDcEQsV0FBVTtrQkFIWjtTQUtFLHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFHO1NBQXlCOzs7OztTQUMxQyx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBUTtTQUF3Qjs7Ozs7U0FDOUMsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQVE7U0FBMEI7Ozs7O1NBQ2hELHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFRO1NBQTRCOzs7OztTQUNsRCx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBYTtTQUE0Qjs7Ozs7UUFDakQ7Ozs7O2VBQ0w7Ozs7O09BR0wsd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFNBQUQ7UUFBTyxXQUFVO2tCQUE0RDtPQUV0RTs7OztpQkFDUCx3QkFBQyxVQUFEO1FBQ0UsT0FBTztRQUNQLFdBQVcsTUFBTSxnQkFBZ0IsRUFBRSxPQUFPLEtBQUs7UUFDL0MsV0FBVTtrQkFIWixDQUtFLHdCQUFDLFVBQUQ7U0FBUSxPQUFNO21CQUFHO1FBQXNCOzs7O2tCQUN0QyxVQUFVLEtBQUssTUFDZCx3QkFBQyxVQUFEO1NBQWdCLE9BQU87bUJBQ3BCO1FBQ0ssR0FGSzs7OztlQUVMLENBQ1QsQ0FDSzs7Ozs7ZUFDTDs7Ozs7TUFDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxPQUFEO09BQUs7T0FDTSx3QkFBQyxVQUFEO1FBQVEsV0FBVTtrQkFBOEIsY0FBYztPQUFlOzs7OztPQUFFO09BQUk7TUFFekY7Ozs7Z0JBQ0wsd0JBQUMsT0FBRDtPQUFLLFdBQVU7aUJBQWYsQ0FBNEMsa0JBQzVCLHdCQUFDLFFBQUQ7UUFBTSxXQUFVO2tCQUF5QjtPQUEwQjs7OztlQUM5RTs7Ozs7Y0FDRjs7Ozs7O0lBQ0Y7Ozs7OztHQUdKLGFBQWEsU0FDWix3QkFBQyxPQUFEO0lBQUssV0FBVTtjQUNaLGNBQWMsS0FBSyxTQUNsQix3QkFBQyxPQUFEO0tBRUUsV0FBVTtlQUZaLENBSUUsd0JBQUMsT0FBRDtNQUVFLHdCQUFDLE9BQUQ7T0FBSyxXQUFVO2lCQUFmLENBQ0Usd0JBQUMsUUFBRDtRQUFNLFdBQVU7a0JBQ2IsS0FBSztPQUNGOzs7O2lCQUNOLHdCQUFDLFFBQUQ7UUFDRSxXQUFXLCtDQUNULEtBQUssZ0JBQWdCLFNBQVMsT0FBTyxJQUNqQyxxREFDQTtrQkFHTCxLQUFLO09BQ0Y7Ozs7ZUFDSDs7Ozs7O01BRUwsd0JBQUMsTUFBRDtPQUFJLFdBQVU7aUJBQ1gsS0FBSztNQUNKOzs7OztNQUNKLHdCQUFDLE9BQUQ7T0FBSyxXQUFVO2lCQUErQyxLQUFLO01BQWdCOzs7OztNQUduRix3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBZjtRQUNFLHdCQUFDLE9BQUQ7U0FBSyxXQUFVO21CQUFmLENBQ0Usd0JBQUMsUUFBRCxFQUFRLFdBQVUsMkNBQTRDOzs7O21CQUM5RCx3QkFBQyxRQUFELGFBQU0sZ0JBQ1Esd0JBQUMsVUFBRCxZQUFTLEtBQUssT0FBZTs7OztpQkFDckM7Ozs7aUJBQ0g7Ozs7OztRQUNMLHdCQUFDLE9BQUQ7U0FBSyxXQUFVO21CQUFmLENBQ0Usd0JBQUMsUUFBRCxFQUFRLFdBQVUsZ0RBQWlEOzs7O21CQUNuRSx3QkFBQyxRQUFEO1VBQU0sV0FBVTtvQkFBZ0IsS0FBSztTQUFjOzs7O2lCQUNoRDs7Ozs7O1FBQ0wsd0JBQUMsT0FBRDtTQUFLLFdBQVU7bUJBQWYsQ0FDRSx3QkFBQyxPQUFELEVBQU8sV0FBVSw2Q0FBOEM7Ozs7bUJBQy9ELHdCQUFDLFFBQUQ7VUFBTSxXQUFVO29CQUE0QixLQUFLO1NBQVk7Ozs7aUJBQzFEOzs7Ozs7UUFDTCx3QkFBQyxPQUFEO1NBQUssV0FBVTttQkFBZjtVQUFxRztVQUMvRixLQUFLLFNBQVMsZUFBZTtVQUFFO1VBQVEsS0FBSyxTQUFTLGVBQWU7VUFBRTtTQUN2RTs7Ozs7O09BQ0Y7Ozs7OztLQUNGOzs7O2VBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxVQUFEO09BQ0UsZUFBZSx1QkFBdUIsSUFBSTtPQUMxQyxXQUFVO2lCQUZaLENBSUUsd0JBQUMsU0FBRCxFQUFTLFdBQVUsY0FBZTs7OztpQkFDbEMsd0JBQUMsUUFBRCxZQUFNLGtCQUFxQjs7OztlQUNyQjs7Ozs7Z0JBQ1Isd0JBQUMsVUFBRDtPQUNFLGVBQWUsZ0JBQWdCLElBQUk7T0FDbkMsT0FBTTtPQUNOLFdBQVU7aUJBRVYsd0JBQUMsV0FBRCxFQUFXLFdBQVUsVUFBVzs7Ozs7TUFDMUI7Ozs7Y0FDTDs7Ozs7YUFDRjtPQWhFRSxLQUFLOzs7O1dBZ0VQLENBQ047R0FDRTs7Ozs7O0dBR0wsd0JBQUMsT0FBRDtJQUFLLFdBQVU7Y0FDYix3QkFBQyxPQUFEO0tBQUssV0FBVTtlQUNiLHdCQUFDLFNBQUQ7TUFBTyxXQUFVO2dCQUFqQixDQUNFLHdCQUFDLFNBQUQ7T0FBTyxXQUFVO2lCQUNmLHdCQUFDLE1BQUQ7UUFDRSx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBTTtRQUFhOzs7OztRQUNqQyx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBTTtRQUFjOzs7OztRQUNsQyx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBTTtRQUFjOzs7OztRQUNsQyx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBTTtRQUFnQjs7Ozs7UUFDcEMsd0JBQUMsTUFBRDtTQUFJLFdBQVU7bUJBQU07UUFBc0I7Ozs7O1FBQzFDLHdCQUFDLE1BQUQ7U0FBSSxXQUFVO21CQUFNO1FBQWtCOzs7OztRQUN0Qyx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBTTtRQUFZOzs7OztRQUNoQyx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBTTtRQUFROzs7OztRQUM1Qix3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFBaUI7UUFBWTs7Ozs7T0FDekM7Ozs7O01BQ0M7Ozs7Z0JBQ1Asd0JBQUMsU0FBRDtPQUFPLFdBQVU7aUJBQ2QsY0FBYyxLQUFLLE1BQ2xCLHdCQUFDLE1BQUQ7UUFBZSxXQUFVO2tCQUF6QjtTQUNFLHdCQUFDLE1BQUQ7VUFBSSxXQUFVO29CQUE0QyxFQUFFO1NBQVM7Ozs7O1NBQ3JFLHdCQUFDLE1BQUQ7VUFBSSxXQUFVO29CQUFkLENBQ0Usd0JBQUMsT0FBRDtXQUFLLFdBQVU7cUJBQXdCLEVBQUU7VUFBVTs7OztvQkFDbkQsd0JBQUMsT0FBRDtXQUFLLFdBQVU7cUJBQThCLEVBQUU7VUFBZ0I7Ozs7a0JBQzdEOzs7Ozs7U0FDSix3QkFBQyxNQUFEO1VBQUksV0FBVTtvQkFBTyxFQUFFO1NBQVc7Ozs7O1NBQ2xDLHdCQUFDLE1BQUQ7VUFBSSxXQUFVO29CQUFPLEVBQUU7U0FBYTs7Ozs7U0FDcEMsd0JBQUMsTUFBRDtVQUFJLFdBQVU7b0JBQWQ7V0FBdUQ7V0FDakQsRUFBRSxTQUFTLGVBQWU7V0FDOUIsd0JBQUMsTUFBRCxDQUFLOzs7OztXQUFDO1dBQ0YsRUFBRSxTQUFTLGVBQWU7VUFDNUI7Ozs7OztTQUNKLHdCQUFDLE1BQUQ7VUFBSSxXQUFVO29CQUFPLEVBQUUsYUFBYTtTQUFROzs7OztTQUM1Qyx3QkFBQyxNQUFEO1VBQUksV0FBVTtvQkFBa0MsRUFBRTtTQUFVOzs7OztTQUM1RCx3QkFBQyxNQUFEO1VBQUksV0FBVTtvQkFDWix3QkFBQyxRQUFEO1dBQ0UsV0FBVywrQ0FDVCxFQUFFLGdCQUFnQixTQUFTLE9BQU8sSUFDOUIsOEJBQ0E7cUJBR0wsRUFBRTtVQUNDOzs7OztTQUNKOzs7OztTQUNKLHdCQUFDLE1BQUQ7VUFBSSxXQUFVO29CQUNaLHdCQUFDLFVBQUQ7V0FDRSxlQUFlLHVCQUF1QixDQUFDO1dBQ3ZDLFdBQVU7cUJBQ1g7VUFFTzs7Ozs7U0FDTjs7Ozs7UUFDRjtVQWxDSyxFQUFFOzs7O2NBa0NQLENBQ0w7TUFDSTs7OztjQUNGOzs7Ozs7SUFDSjs7Ozs7R0FDRjs7Ozs7RUFFSjs7Ozs7O0FBRVQiLCJuYW1lcyI6W10sInNvdXJjZXMiOlsiVW5pdFNlYXJjaFZpZXcudHN4Il0sInZlcnNpb24iOjMsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBSZWFjdCwgeyB1c2VTdGF0ZSwgdXNlTWVtbyB9IGZyb20gJ3JlYWN0JztcbmltcG9ydCB7IE1pbGl0YXJ5VW5pdCB9IGZyb20gJy4uL3R5cGVzL2dpcyc7XG5pbXBvcnQgeyBIQU5PSV9ESVNUUklDVFMgfSBmcm9tICcuLi9kYXRhL2hhbm9pRGlzdHJpY3RzJztcbmltcG9ydCB7XG4gIFNlYXJjaCxcbiAgRmlsdGVyLFxuICBNYXBQaW4sXG4gIENvbXBhc3MsXG4gIFBob25lLFxuICBTaGllbGQsXG4gIEFjdGl2aXR5LFxuICBMYXllcnMsXG4gIExheW91dEdyaWQsXG4gIExpc3QsXG4gIFJvdGF0ZUNjdyxcbiAgQ2lyY2xlRG90LFxuICBFeWUsXG59IGZyb20gJ2x1Y2lkZS1yZWFjdCc7XG5cbmludGVyZmFjZSBVbml0U2VhcmNoVmlld1Byb3BzIHtcbiAgdW5pdHM6IE1pbGl0YXJ5VW5pdFtdO1xuICBvblNlbGVjdFVuaXRBbmRHb1RvTWFwOiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xuICBvblRyaWdnZXJCdWZmZXI6ICh1bml0OiBNaWxpdGFyeVVuaXQpID0+IHZvaWQ7XG59XG5cbmV4cG9ydCBjb25zdCBVbml0U2VhcmNoVmlldzogUmVhY3QuRkM8VW5pdFNlYXJjaFZpZXdQcm9wcz4gPSAoe1xuICB1bml0cyxcbiAgb25TZWxlY3RVbml0QW5kR29Ub01hcCxcbiAgb25UcmlnZ2VyQnVmZmVyLFxufSkgPT4ge1xuICBjb25zdCBbc2VhcmNoVGVybSwgc2V0U2VhcmNoVGVybV0gPSB1c2VTdGF0ZSgnJyk7XG4gIGNvbnN0IFtzZWxlY3RlZEJyYW5jaCwgc2V0U2VsZWN0ZWRCcmFuY2hdID0gdXNlU3RhdGUoJycpO1xuICBjb25zdCBbc2VsZWN0ZWRUeXBlLCBzZXRTZWxlY3RlZFR5cGVdID0gdXNlU3RhdGUoJycpO1xuICBjb25zdCBbc2VsZWN0ZWREaXN0cmljdCwgc2V0U2VsZWN0ZWREaXN0cmljdF0gPSB1c2VTdGF0ZSgnJyk7XG4gIGNvbnN0IFtzZWxlY3RlZFJlYWRpbmVzcywgc2V0U2VsZWN0ZWRSZWFkaW5lc3NdID0gdXNlU3RhdGUoJycpO1xuICBjb25zdCBbc2VsZWN0ZWRTZWN1cml0eSwgc2V0U2VsZWN0ZWRTZWN1cml0eV0gPSB1c2VTdGF0ZSgnJyk7XG4gIGNvbnN0IFt2aWV3TW9kZSwgc2V0Vmlld01vZGVdID0gdXNlU3RhdGU8J2dyaWQnIHwgJ3RhYmxlJz4oJ2dyaWQnKTtcblxuICAvLyBVbmlxdWUgb3B0aW9uc1xuICBjb25zdCBicmFuY2hlcyA9IHVzZU1lbW8oKCkgPT4gQXJyYXkuZnJvbShuZXcgU2V0KHVuaXRzLm1hcCgodSkgPT4gdS5icmFuY2gpKSkuc29ydCgpLCBbdW5pdHNdKTtcbiAgY29uc3QgdW5pdFR5cGVzID0gdXNlTWVtbygoKSA9PiBBcnJheS5mcm9tKG5ldyBTZXQodW5pdHMubWFwKCh1KSA9PiB1LnVuaXRUeXBlKSkpLnNvcnQoKSwgW3VuaXRzXSk7XG5cbiAgLy8gRmlsdGVyZWQgdW5pdHNcbiAgY29uc3QgZmlsdGVyZWRVbml0cyA9IHVzZU1lbW8oKCkgPT4ge1xuICAgIHJldHVybiB1bml0cy5maWx0ZXIoKHUpID0+IHtcbiAgICAgIGNvbnN0IG1hdGNoU2VhcmNoID1cbiAgICAgICAgc2VhcmNoVGVybSA9PT0gJycgfHxcbiAgICAgICAgdS5uYW1lLnRvTG93ZXJDYXNlKCkuaW5jbHVkZXMoc2VhcmNoVGVybS50b0xvd2VyQ2FzZSgpKSB8fFxuICAgICAgICB1LmNvZGUudG9Mb3dlckNhc2UoKS5pbmNsdWRlcyhzZWFyY2hUZXJtLnRvTG93ZXJDYXNlKCkpIHx8XG4gICAgICAgIHUuc2hvcnROYW1lLnRvTG93ZXJDYXNlKCkuaW5jbHVkZXMoc2VhcmNoVGVybS50b0xvd2VyQ2FzZSgpKSB8fFxuICAgICAgICB1LmFkZHJlc3MudG9Mb3dlckNhc2UoKS5pbmNsdWRlcyhzZWFyY2hUZXJtLnRvTG93ZXJDYXNlKCkpIHx8XG4gICAgICAgIHUuY29tbWFuZGVyLnRvTG93ZXJDYXNlKCkuaW5jbHVkZXMoc2VhcmNoVGVybS50b0xvd2VyQ2FzZSgpKTtcblxuICAgICAgY29uc3QgbWF0Y2hCcmFuY2ggPSBzZWxlY3RlZEJyYW5jaCA9PT0gJycgfHwgdS5icmFuY2ggPT09IHNlbGVjdGVkQnJhbmNoO1xuICAgICAgY29uc3QgbWF0Y2hUeXBlID0gc2VsZWN0ZWRUeXBlID09PSAnJyB8fCB1LnVuaXRUeXBlID09PSBzZWxlY3RlZFR5cGU7XG4gICAgICBjb25zdCBtYXRjaERpc3RyaWN0ID0gc2VsZWN0ZWREaXN0cmljdCA9PT0gJycgfHwgdS5kaXN0cmljdCA9PT0gc2VsZWN0ZWREaXN0cmljdDtcbiAgICAgIGNvbnN0IG1hdGNoUmVhZGluZXNzID0gc2VsZWN0ZWRSZWFkaW5lc3MgPT09ICcnIHx8IHUucmVhZGluZXNzU3RhdHVzLmluY2x1ZGVzKHNlbGVjdGVkUmVhZGluZXNzKTtcbiAgICAgIGNvbnN0IG1hdGNoU2VjdXJpdHkgPSBzZWxlY3RlZFNlY3VyaXR5ID09PSAnJyB8fCB1LnNlY3VyaXR5TGV2ZWwgPT09IHNlbGVjdGVkU2VjdXJpdHk7XG5cbiAgICAgIHJldHVybiAoXG4gICAgICAgIG1hdGNoU2VhcmNoICYmIG1hdGNoQnJhbmNoICYmIG1hdGNoVHlwZSAmJiBtYXRjaERpc3RyaWN0ICYmIG1hdGNoUmVhZGluZXNzICYmIG1hdGNoU2VjdXJpdHlcbiAgICAgICk7XG4gICAgfSk7XG4gIH0sIFtcbiAgICB1bml0cyxcbiAgICBzZWFyY2hUZXJtLFxuICAgIHNlbGVjdGVkQnJhbmNoLFxuICAgIHNlbGVjdGVkVHlwZSxcbiAgICBzZWxlY3RlZERpc3RyaWN0LFxuICAgIHNlbGVjdGVkUmVhZGluZXNzLFxuICAgIHNlbGVjdGVkU2VjdXJpdHksXG4gIF0pO1xuXG4gIGNvbnN0IGhhbmRsZVJlc2V0RmlsdGVycyA9ICgpID0+IHtcbiAgICBzZXRTZWFyY2hUZXJtKCcnKTtcbiAgICBzZXRTZWxlY3RlZEJyYW5jaCgnJyk7XG4gICAgc2V0U2VsZWN0ZWRUeXBlKCcnKTtcbiAgICBzZXRTZWxlY3RlZERpc3RyaWN0KCcnKTtcbiAgICBzZXRTZWxlY3RlZFJlYWRpbmVzcygnJyk7XG4gICAgc2V0U2VsZWN0ZWRTZWN1cml0eSgnJyk7XG4gIH07XG5cbiAgcmV0dXJuIChcbiAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXgtMSBvdmVyZmxvdy15LWF1dG8gYmctWyMwODBkMTldIHRleHQtd2hpdGUgcC02IHNwYWNlLXktNlwiPlxuICAgICAgey8qIEhlYWRlciAmIFRpdGxlICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGZsZXgtd3JhcCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGdhcC00IGJvcmRlci1iIGJvcmRlci1zbGF0ZS04MDAgcGItNFwiPlxuICAgICAgICA8ZGl2PlxuICAgICAgICAgIDxoMiBjbGFzc05hbWU9XCJ0ZXh0LXhsIGZvbnQtYm9sZCB1cHBlcmNhc2UgdHJhY2tpbmctd2lkZXIgdGV4dC13aGl0ZSBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgPFNlYXJjaCBjbGFzc05hbWU9XCJ3LTUgaC01IHRleHQtZW1lcmFsZC00MDBcIiAvPlxuICAgICAgICAgICAgPHNwYW4+VHJhIGPhu6l1IFRow7RuZyB0aW4gxJDGoW4gduG7iyBRdcOibiBz4buxPC9zcGFuPlxuICAgICAgICAgIDwvaDI+XG4gICAgICAgICAgPHAgY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LXNsYXRlLTQwMCBtdC0xXCI+XG4gICAgICAgICAgICBC4buZIGzhu41jIMSRYSB0acOqdSBjaMOtIHRoZW8ga2jDtG5nIGdpYW4gaMOgbmggY2jDrW5oLCBxdcOibiBiaW5oIGNo4bunbmcgdsOgIHRy4bqhbmcgdGjDoWkgc+G6tW4gc8OgbmdcbiAgICAgICAgICAgIGNoaeG6v24gxJHhuqV1XG4gICAgICAgICAgPC9wPlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogVmlldyBtb2RlIHRvZ2dsZSAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBiZy1zbGF0ZS05MDAgYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBwLTFcIj5cbiAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRWaWV3TW9kZSgnZ3JpZCcpfVxuICAgICAgICAgICAgY2xhc3NOYW1lPXtgcC0xLjUgcm91bmRlZCB0ZXh0LXhzIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xIHRyYW5zaXRpb24gJHtcbiAgICAgICAgICAgICAgdmlld01vZGUgPT09ICdncmlkJ1xuICAgICAgICAgICAgICAgID8gJ2JnLWVtZXJhbGQtNjAwIHRleHQtd2hpdGUgZm9udC1zZW1pYm9sZCdcbiAgICAgICAgICAgICAgICA6ICd0ZXh0LXNsYXRlLTQwMCBob3Zlcjp0ZXh0LXdoaXRlJ1xuICAgICAgICAgICAgfWB9XG4gICAgICAgICAgPlxuICAgICAgICAgICAgPExheW91dEdyaWQgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjVcIiAvPlxuICAgICAgICAgICAgPHNwYW4+ROG6oW5nIFRo4bq7PC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldFZpZXdNb2RlKCd0YWJsZScpfVxuICAgICAgICAgICAgY2xhc3NOYW1lPXtgcC0xLjUgcm91bmRlZCB0ZXh0LXhzIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xIHRyYW5zaXRpb24gJHtcbiAgICAgICAgICAgICAgdmlld01vZGUgPT09ICd0YWJsZSdcbiAgICAgICAgICAgICAgICA/ICdiZy1lbWVyYWxkLTYwMCB0ZXh0LXdoaXRlIGZvbnQtc2VtaWJvbGQnXG4gICAgICAgICAgICAgICAgOiAndGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC13aGl0ZSdcbiAgICAgICAgICAgIH1gfVxuICAgICAgICAgID5cbiAgICAgICAgICAgIDxMaXN0IGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgIDxzcGFuPkThuqFuZyBC4bqjbmcgR0lTPC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8L2Rpdj5cbiAgICAgIDwvZGl2PlxuXG4gICAgICB7LyogTXVsdGktY3JpdGVyaWEgRmlsdGVyIEJhciAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC00IHNoYWRvdy1tZCBzcGFjZS15LTNcIj5cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW5cIj5cbiAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXhzIGZvbnQtYm9sZCB1cHBlcmNhc2UgdGV4dC1lbWVyYWxkLTQwMCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICA8RmlsdGVyIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgIDxzcGFuPkLhu5ggTOG7jEMgVMOMTSBLSeG6vk0gxJBBIFRJw4pVIENIw408L3NwYW4+XG4gICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZVJlc2V0RmlsdGVyc31cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC1yb3NlLTQwMCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMSB0cmFuc2l0aW9uXCJcbiAgICAgICAgICA+XG4gICAgICAgICAgICA8Um90YXRlQ2N3IGNsYXNzTmFtZT1cInctMyBoLTNcIiAvPlxuICAgICAgICAgICAgPHNwYW4+xJDhurd0IGzhuqFpIGLhu5kgbOG7jWM8L3NwYW4+XG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMSBzbTpncmlkLWNvbHMtMiBsZzpncmlkLWNvbHMtNiBnYXAtM1wiPlxuICAgICAgICAgIHsvKiBLZXl3b3JkIFNlYXJjaCAqL31cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImxnOmNvbC1zcGFuLTJcIj5cbiAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgVOG7qyBraMOzYSB0w6xtIGtp4bq/bVxuICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicmVsYXRpdmVcIj5cbiAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgdHlwZT1cInRleHRcIlxuICAgICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiVMOqbiwgbcOjIHPhu5EsIMSR4buLYSBjaOG7iSwgY2jhu4kgaHV5Li4uXCJcbiAgICAgICAgICAgICAgICB2YWx1ZT17c2VhcmNoVGVybX1cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFNlYXJjaFRlcm0oZS50YXJnZXQudmFsdWUpfVxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBweC0zIHB5LTIgdGV4dC14cyB0ZXh0LXdoaXRlIHBsYWNlaG9sZGVyLXNsYXRlLTUwMCBmb2N1czpvdXRsaW5lLW5vbmUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwIHBsLThcIlxuICAgICAgICAgICAgICAvPlxuICAgICAgICAgICAgICA8U2VhcmNoIGNsYXNzTmFtZT1cInctMy41IGgtMy41IHRleHQtc2xhdGUtNDAwIGFic29sdXRlIGxlZnQtMi41IHRvcC0zXCIgLz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIEJyYW5jaCAqL31cbiAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSBmb250LWJvbGQgYmxvY2sgbWItMVwiPlxuICAgICAgICAgICAgICBRdcOibiBCaW5oIGNo4bunbmdcbiAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgIHZhbHVlPXtzZWxlY3RlZEJyYW5jaH1cbiAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRTZWxlY3RlZEJyYW5jaChlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBweC0yLjUgcHktMiB0ZXh0LXhzIHRleHQtd2hpdGUgZm9jdXM6b3V0bGluZS1ub25lIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICA+XG4gICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJcIj5U4bqldCBj4bqjIEJpbmggY2jhu6duZzwvb3B0aW9uPlxuICAgICAgICAgICAgICB7YnJhbmNoZXMubWFwKChiKSA9PiAoXG4gICAgICAgICAgICAgICAgPG9wdGlvbiBrZXk9e2J9IHZhbHVlPXtifT5cbiAgICAgICAgICAgICAgICAgIHtifVxuICAgICAgICAgICAgICAgIDwvb3B0aW9uPlxuICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgIDwvc2VsZWN0PlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIERpc3RyaWN0ICovfVxuICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgIMSQ4buLYSBiw6BuIFF14bqtbiAvIEh1eeG7h25cbiAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgIHZhbHVlPXtzZWxlY3RlZERpc3RyaWN0fVxuICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFNlbGVjdGVkRGlzdHJpY3QoZS50YXJnZXQudmFsdWUpfVxuICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwOTBmMWRdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQtbGcgcHgtMi41IHB5LTIgdGV4dC14cyB0ZXh0LXdoaXRlIGZvY3VzOm91dGxpbmUtbm9uZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDBcIlxuICAgICAgICAgICAgPlxuICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiXCI+VOG6pXQgY+G6oyAzMCBRdeG6rW4vSHV54buHbjwvb3B0aW9uPlxuICAgICAgICAgICAgICB7SEFOT0lfRElTVFJJQ1RTLm1hcCgoZCkgPT4gKFxuICAgICAgICAgICAgICAgIDxvcHRpb24ga2V5PXtkLmNvZGV9IHZhbHVlPXtkLm5hbWV9PlxuICAgICAgICAgICAgICAgICAge2QubmFtZX1cbiAgICAgICAgICAgICAgICA8L29wdGlvbj5cbiAgICAgICAgICAgICAgKSl9XG4gICAgICAgICAgICA8L3NlbGVjdD5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBSZWFkaW5lc3MgKi99XG4gICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgVHLhuqFuZyB0aMOhaSBTU0PEkFxuICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgIDxzZWxlY3RcbiAgICAgICAgICAgICAgdmFsdWU9e3NlbGVjdGVkUmVhZGluZXNzfVxuICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFNlbGVjdGVkUmVhZGluZXNzKGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidy1mdWxsIGJnLVsjMDkwZjFkXSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkLWxnIHB4LTIuNSBweS0yIHRleHQteHMgdGV4dC13aGl0ZSBmb2N1czpvdXRsaW5lLW5vbmUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwXCJcbiAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlwiPlThuqV0IGPhuqMgdHLhuqFuZyB0aMOhaTwvb3B0aW9uPlxuICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiQ+G6pXAgMVwiPkPhuqVwIDEgKENhbyBuaOG6pXQpPC9vcHRpb24+XG4gICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJD4bqlcCAyXCI+Q+G6pXAgMiAoVMSDbmcgY8aw4budbmcpPC9vcHRpb24+XG4gICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJD4bqlcCAzXCI+Q+G6pXAgMyAoVGjGsOG7nW5nIHh1ecOqbik8L29wdGlvbj5cbiAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkh14bqlbiBsdXnhu4duXCI+SHXhuqVuIGx1eeG7h24gLSDEkMOgbyB04bqhbzwvb3B0aW9uPlxuICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICB7LyogVW5pdCBUeXBlICovfVxuICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgIEPhuqVwIC8gTG/huqFpIGjDrG5oXG4gICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgPHNlbGVjdFxuICAgICAgICAgICAgICB2YWx1ZT17c2VsZWN0ZWRUeXBlfVxuICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFNlbGVjdGVkVHlwZShlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBweC0yLjUgcHktMiB0ZXh0LXhzIHRleHQtd2hpdGUgZm9jdXM6b3V0bGluZS1ub25lIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICA+XG4gICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJcIj5U4bqldCBj4bqjIGPDoWMgY+G6pXA8L29wdGlvbj5cbiAgICAgICAgICAgICAge3VuaXRUeXBlcy5tYXAoKHQpID0+IChcbiAgICAgICAgICAgICAgICA8b3B0aW9uIGtleT17dH0gdmFsdWU9e3R9PlxuICAgICAgICAgICAgICAgICAge3R9XG4gICAgICAgICAgICAgICAgPC9vcHRpb24+XG4gICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHsvKiBSZXN1bHRzIENvdW50ZXIgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gaXRlbXMtY2VudGVyIHRleHQteHMgdGV4dC1zbGF0ZS00MDAgcHQtMiBib3JkZXItdCBib3JkZXItc2xhdGUtODAwXCI+XG4gICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgIFTDrG0gdGjhuqV5IDxzdHJvbmcgY2xhc3NOYW1lPVwidGV4dC1lbWVyYWxkLTQwMCBmb250LW1vbm9cIj57ZmlsdGVyZWRVbml0cy5sZW5ndGh9PC9zdHJvbmc+eycgJ31cbiAgICAgICAgICAgIGvhur90IHF14bqjIHBow7kgaOG7o3BcbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtWzExcHhdIHRleHQtc2xhdGUtNDAwXCI+XG4gICAgICAgICAgICBI4buHIHF1eSBjaGnhur91OiA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC1za3ktNDAwXCI+Vk4tMjAwMCAvIEVQU0c6MzI2NDg8L3NwYW4+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBHcmlkIFZpZXcgKi99XG4gICAgICB7dmlld01vZGUgPT09ICdncmlkJyA/IChcbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIG1kOmdyaWQtY29scy0yIGxnOmdyaWQtY29scy0zIGdhcC00XCI+XG4gICAgICAgICAge2ZpbHRlcmVkVW5pdHMubWFwKCh1bml0KSA9PiAoXG4gICAgICAgICAgICA8ZGl2XG4gICAgICAgICAgICAgIGtleT17dW5pdC5pZH1cbiAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIGhvdmVyOmJvcmRlci1lbWVyYWxkLTYwMCByb3VuZGVkLXhsIHAtNCBzaGFkb3ctbWQgdHJhbnNpdGlvbi1hbGwgZmxleCBmbGV4LWNvbCBqdXN0aWZ5LWJldHdlZW5cIlxuICAgICAgICAgICAgPlxuICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgIHsvKiBCYWRnZXMgKi99XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gZ2FwLTIgbWItMlwiPlxuICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwiZm9udC1tb25vIHRleHQteHMgcHgtMiBweS0wLjUgcm91bmRlZCBiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwIGJvcmRlciBib3JkZXItZW1lcmFsZC04MDAgZm9udC1ib2xkXCI+XG4gICAgICAgICAgICAgICAgICAgIHt1bml0LmNvZGV9XG4gICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8c3BhblxuICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9e2B0ZXh0LVsxMHB4XSBweC0yIHB5LTAuNSByb3VuZGVkIGZvbnQtbWVkaXVtICR7XG4gICAgICAgICAgICAgICAgICAgICAgdW5pdC5yZWFkaW5lc3NTdGF0dXMuaW5jbHVkZXMoJ0PhuqVwIDEnKVxuICAgICAgICAgICAgICAgICAgICAgICAgPyAnYmctcm9zZS05NTAgdGV4dC1yb3NlLTMwMCBib3JkZXIgYm9yZGVyLXJvc2UtODAwJ1xuICAgICAgICAgICAgICAgICAgICAgICAgOiAnYmctZW1lcmFsZC05NTAgdGV4dC1lbWVyYWxkLTMwMCBib3JkZXIgYm9yZGVyLWVtZXJhbGQtODAwJ1xuICAgICAgICAgICAgICAgICAgICB9YH1cbiAgICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgICAge3VuaXQucmVhZGluZXNzU3RhdHVzfVxuICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgPGgzIGNsYXNzTmFtZT1cInRleHQtc20gZm9udC1ib2xkIHRleHQtd2hpdGUgbGluZS1jbGFtcC0yIGxlYWRpbmctc251Z1wiPlxuICAgICAgICAgICAgICAgICAge3VuaXQubmFtZX1cbiAgICAgICAgICAgICAgICA8L2gzPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMSBmb250LW1lZGl1bVwiPnt1bml0LnBhcmVudFVuaXR9PC9kaXY+XG5cbiAgICAgICAgICAgICAgICB7LyogRGV0YWlscyAqL31cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cIm10LTMgc3BhY2UteS0xLjUgdGV4dC14cyB0ZXh0LXNsYXRlLTMwMFwiPlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICAgICAgICAgIDxTaGllbGQgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjUgdGV4dC1hbWJlci00MDAgZmxleC1zaHJpbmstMFwiIC8+XG4gICAgICAgICAgICAgICAgICAgIDxzcGFuPlxuICAgICAgICAgICAgICAgICAgICAgIEJpbmggY2jhu6duZzogPHN0cm9uZz57dW5pdC5icmFuY2h9PC9zdHJvbmc+XG4gICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLXN0YXJ0IGdhcC0xLjVcIj5cbiAgICAgICAgICAgICAgICAgICAgPE1hcFBpbiBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LXNreS00MDAgZmxleC1zaHJpbmstMCBtdC0wLjVcIiAvPlxuICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJsaW5lLWNsYW1wLTFcIj57dW5pdC5hZGRyZXNzfTwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICAgICAgICAgIDxQaG9uZSBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LWVtZXJhbGQtNDAwIGZsZXgtc2hyaW5rLTBcIiAvPlxuICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC1zbGF0ZS0zMDBcIj57dW5pdC5waG9uZX08L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC0yIGJnLVsjMDkwZjFkXSByb3VuZGVkIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHRleHQtWzEwcHhdIGZvbnQtbW9ubyB0ZXh0LXNreS0zMDBcIj5cbiAgICAgICAgICAgICAgICAgICAgWDoge3VuaXQueF92bjIwMDAudG9Mb2NhbGVTdHJpbmcoKX1tIHwgWToge3VuaXQueV92bjIwMDAudG9Mb2NhbGVTdHJpbmcoKX1tXG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgey8qIEFjdGlvbiBidXR0b25zICovfVxuICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cIm10LTQgcHQtMyBib3JkZXItdCBib3JkZXItc2xhdGUtODAwIGZsZXggZ2FwLTJcIj5cbiAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBvblNlbGVjdFVuaXRBbmRHb1RvTWFwKHVuaXQpfVxuICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiZmxleC0xIGJnLWVtZXJhbGQtNjAwIGhvdmVyOmJnLWVtZXJhbGQtNTAwIHRleHQtd2hpdGUgdGV4dC14cyBmb250LXNlbWlib2xkIHB5LTIgcHgtMyByb3VuZGVkIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIGdhcC0xLjUgc2hhZG93IHRyYW5zaXRpb25cIlxuICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgIDxDb21wYXNzIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgICAgICAgIDxzcGFuPlhlbSB0csOqbiBi4bqjbiDEkeG7kzwvc3Bhbj5cbiAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBvblRyaWdnZXJCdWZmZXIodW5pdCl9XG4gICAgICAgICAgICAgICAgICB0aXRsZT1cIlThuqFvIHbDuW5nIMSR4buHbSBwaMOybmcgdGjhu6cgYsOhbiBrw61uaCAxMGttXCJcbiAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLXNsYXRlLTgwMCBob3ZlcjpiZy1hbWJlci03MDAgdGV4dC1zbGF0ZS0zMDAgaG92ZXI6dGV4dC13aGl0ZSB0ZXh0LXhzIHAtMiByb3VuZGVkIHRyYW5zaXRpb25cIlxuICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgIDxDaXJjbGVEb3QgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgKSl9XG4gICAgICAgIDwvZGl2PlxuICAgICAgKSA6IChcbiAgICAgICAgLyogVGFibGUgVmlldyAqL1xuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIG92ZXJmbG93LWhpZGRlbiBzaGFkb3ctbGdcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cIm92ZXJmbG93LXgtYXV0b1wiPlxuICAgICAgICAgICAgPHRhYmxlIGNsYXNzTmFtZT1cInctZnVsbCB0ZXh0LWxlZnQgdGV4dC14cyB0ZXh0LXNsYXRlLTMwMFwiPlxuICAgICAgICAgICAgICA8dGhlYWQgY2xhc3NOYW1lPVwiYmctWyMwYjEyMjBdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSB0ZXh0LVsxMHB4XSB0cmFja2luZy13aWRlciBmb250LW1vbm8gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMFwiPlxuICAgICAgICAgICAgICAgIDx0cj5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5Nw6MgxJHGoW4gduG7izwvdGg+XG4gICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zXCI+VMOqbiDEkcahbiB24buLPC90aD5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5CaW5oIGNo4bunbmc8L3RoPlxuICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPlF14bqtbiAvIEh1eeG7h248L3RoPlxuICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPlThu41hIMSR4buZIFZOLTIwMDAgKG0pPC90aD5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5DaOG7iSBodXkgdHLGsOG7n25nPC90aD5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5UcuG7sWMgYmFuPC90aD5cbiAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5TU0PEkDwvdGg+XG4gICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zIHRleHQtcmlnaHRcIj5UaGFvIHTDoWM8L3RoPlxuICAgICAgICAgICAgICAgIDwvdHI+XG4gICAgICAgICAgICAgIDwvdGhlYWQ+XG4gICAgICAgICAgICAgIDx0Ym9keSBjbGFzc05hbWU9XCJkaXZpZGUteSBkaXZpZGUtc2xhdGUtODAwXCI+XG4gICAgICAgICAgICAgICAge2ZpbHRlcmVkVW5pdHMubWFwKCh1KSA9PiAoXG4gICAgICAgICAgICAgICAgICA8dHIga2V5PXt1LmlkfSBjbGFzc05hbWU9XCJob3ZlcjpiZy1zbGF0ZS04MDAvNTAgdHJhbnNpdGlvblwiPlxuICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtbW9ubyBmb250LWJvbGQgdGV4dC1lbWVyYWxkLTQwMFwiPnt1LmNvZGV9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtM1wiPlxuICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZm9udC1ib2xkIHRleHQtd2hpdGVcIj57dS5uYW1lfTwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDBcIj57dS5wYXJlbnRVbml0fTwvZGl2PlxuICAgICAgICAgICAgICAgICAgICA8L3RkPlxuICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zXCI+e3UuYnJhbmNofTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTNcIj57dS5kaXN0cmljdH08L3RkPlxuICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtbW9ubyB0ZXh0LVsxMXB4XSB0ZXh0LXNreS0zMDBcIj5cbiAgICAgICAgICAgICAgICAgICAgICBYOiB7dS54X3ZuMjAwMC50b0xvY2FsZVN0cmluZygpfVxuICAgICAgICAgICAgICAgICAgICAgIDxiciAvPlxuICAgICAgICAgICAgICAgICAgICAgIFk6IHt1Lnlfdm4yMDAwLnRvTG9jYWxlU3RyaW5nKCl9XG4gICAgICAgICAgICAgICAgICAgIDwvdGQ+XG4gICAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTNcIj57dS5jb21tYW5kZXIgfHwgJy0nfTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTMgZm9udC1tb25vIHRleHQtZW1lcmFsZC0zMDBcIj57dS5waG9uZX08L3RkPlxuICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW5cbiAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT17YHRleHQtWzEwcHhdIHB4LTIgcHktMC41IHJvdW5kZWQgZm9udC1tZWRpdW0gJHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgdS5yZWFkaW5lc3NTdGF0dXMuaW5jbHVkZXMoJ0PhuqVwIDEnKVxuICAgICAgICAgICAgICAgICAgICAgICAgICAgID8gJ2JnLXJvc2UtOTUwIHRleHQtcm9zZS0zMDAnXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgOiAnYmctZW1lcmFsZC05NTAgdGV4dC1lbWVyYWxkLTMwMCdcbiAgICAgICAgICAgICAgICAgICAgICAgIH1gfVxuICAgICAgICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgICAgICAgIHt1LnJlYWRpbmVzc1N0YXR1c31cbiAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgIDwvdGQ+XG4gICAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTMgdGV4dC1yaWdodFwiPlxuICAgICAgICAgICAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IG9uU2VsZWN0VW5pdEFuZEdvVG9NYXAodSl9XG4gICAgICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJiZy1lbWVyYWxkLTcwMC84MCBob3ZlcjpiZy1lbWVyYWxkLTYwMCB0ZXh0LXdoaXRlIHB4LTIuNSBweS0xIHJvdW5kZWQgdGV4dC1bMTFweF0gZm9udC1zZW1pYm9sZFwiXG4gICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAgWGVtIEdJU1xuICAgICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICA8L3RkPlxuICAgICAgICAgICAgICAgICAgPC90cj5cbiAgICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgICAgPC90Ym9keT5cbiAgICAgICAgICAgIDwvdGFibGU+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuICAgICAgKX1cbiAgICA8L2Rpdj5cbiAgKTtcbn07XG4iXX0=