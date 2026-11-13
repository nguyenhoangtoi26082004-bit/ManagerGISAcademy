const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"];const _jsxDEV = __vite__cjsImport4_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { HANOI_DISTRICTS } from "/src/data/hanoiDistricts.ts";
import { wgs84ToUtm48N, utm48NToWgs84 } from "/src/utils/coordinateConversion.ts";
import { X, Save, Compass, Building } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/UnitModal.tsx";
import __vite__cjsImport4_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const UnitModal = ({ isOpen, onClose, onSave, initialUnit }) => {
	if (!isOpen) return null;
	const [code, setCode] = useState(initialUnit?.code || "");
	const [name, setName] = useState(initialUnit?.name || "");
	const [shortName, setShortName] = useState(initialUnit?.shortName || "");
	const [unitType, setUnitType] = useState(initialUnit?.unitType || "Ban Chỉ huy Quân sự");
	const [branch, setBranch] = useState(initialUnit?.branch || "Lục quân");
	const [parentUnit, setParentUnit] = useState(initialUnit?.parentUnit || "Bộ Tư lệnh Thủ đô Hà Nội");
	const [district, setDistrict] = useState(initialUnit?.district || "Quận Ba Đình");
	const [ward, setWard] = useState(initialUnit?.ward || "");
	const [address, setAddress] = useState(initialUnit?.address || "");
	// Coordinates
	const [lat, setLat] = useState(initialUnit?.latitude || 21.034);
	const [lng, setLng] = useState(initialUnit?.longitude || 105.83);
	const [utmX, setUtmX] = useState(initialUnit?.x_vn2000 || 586e3);
	const [utmY, setUtmY] = useState(initialUnit?.y_vn2000 || 2326e3);
	// Command & Readiness
	const [commander, setCommander] = useState(initialUnit?.commander || "");
	const [politicalCommissar, setPoliticalCommissar] = useState(initialUnit?.politicalCommissar || "");
	const [phone, setPhone] = useState(initialUnit?.phone || "024.");
	const [securityLevel, setSecurityLevel] = useState(initialUnit?.securityLevel || "Mật");
	const [readinessStatus, setReadinessStatus] = useState(initialUnit?.readinessStatus || "Cấp 1 (Cao nhất)");
	const [personnelScale, setPersonnelScale] = useState(initialUnit?.personnelScale || "");
	const [missions, setMissions] = useState(initialUnit?.mainMissions.join("; ") || "");
	const [equipment, setEquipment] = useState(initialUnit?.equipmentHighlights.join("; ") || "");
	const [establishedYear, setEstablishedYear] = useState(initialUnit?.establishedYear || 1954);
	const [notes, setNotes] = useState(initialUnit?.notes || "");
	// Auto sync WGS84 -> UTM 48N
	const handleLatChange = (val) => {
		setLat(val);
		const converted = wgs84ToUtm48N(val, lng);
		setUtmX(converted.x);
		setUtmY(converted.y);
	};
	const handleLngChange = (val) => {
		setLng(val);
		const converted = wgs84ToUtm48N(lat, val);
		setUtmX(converted.x);
		setUtmY(converted.y);
	};
	const handleUtmXChange = (val) => {
		setUtmX(val);
		const converted = utm48NToWgs84(val, utmY);
		setLat(converted.lat);
		setLng(converted.lng);
	};
	const handleUtmYChange = (val) => {
		setUtmY(val);
		const converted = utm48NToWgs84(utmX, val);
		setLat(converted.lat);
		setLng(converted.lng);
	};
	const handleSubmit = (e) => {
		e.preventDefault();
		const unit = {
			id: initialUnit?.id || `DV_${Date.now()}`,
			code: code.trim(),
			name: name.trim(),
			shortName: shortName.trim() || name.trim(),
			unitType,
			branch,
			parentUnit: parentUnit.trim(),
			district,
			ward: ward.trim(),
			address: address.trim(),
			latitude: Number(lat),
			longitude: Number(lng),
			x_vn2000: Number(utmX),
			y_vn2000: Number(utmY),
			commander: commander.trim(),
			politicalCommissar: politicalCommissar.trim(),
			phone: phone.trim(),
			securityLevel,
			readinessStatus,
			personnelScale: personnelScale.trim(),
			mainMissions: missions.split(";").map((m) => m.trim()).filter((m) => m.length > 0),
			equipmentHighlights: equipment.split(";").map((e) => e.trim()).filter((e) => e.length > 0),
			establishedYear: Number(establishedYear) || 1954,
			notes: notes.trim(),
			updatedAt: new Date().toISOString()
		};
		onSave(unit);
		onClose();
	};
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "fixed inset-0 z-[2000] flex items-center justify-center bg-black/75 backdrop-blur-sm p-4 overflow-y-auto",
		children: /* @__PURE__ */ _jsxDEV("div", {
			className: "relative w-full max-w-3xl bg-[#09101f] border border-slate-700 rounded-xl shadow-2xl text-white overflow-hidden my-6",
			children: [/* @__PURE__ */ _jsxDEV("div", {
				className: "p-4 bg-gradient-to-r from-slate-900 to-slate-800 border-b border-slate-700 flex items-center justify-between",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "flex items-center gap-2",
					children: [/* @__PURE__ */ _jsxDEV(Building, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 128,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("h3", {
						className: "text-base font-bold text-white",
						children: initialUnit ? "Chỉnh sửa Thông tin Đơn vị Quân sự" : "Thêm Mới Đơn vị Quân đội trên Hà Nội"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 129,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 127,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("button", {
					onClick: onClose,
					className: "text-slate-400 hover:text-white p-1",
					children: /* @__PURE__ */ _jsxDEV(X, { className: "w-5 h-5" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 134,
						columnNumber: 13
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 133,
					columnNumber: 11
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 126,
				columnNumber: 9
			}, this), /* @__PURE__ */ _jsxDEV("form", {
				onSubmit: handleSubmit,
				className: "p-6 space-y-4 max-h-[80vh] overflow-y-auto text-xs",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-3 gap-3",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
							className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
							children: "Mã đơn vị (Code)*:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 143,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("input", {
							type: "text",
							required: true,
							placeholder: "VD: BCH-BD, SD-361",
							value: code,
							onChange: (e) => setCode(e.target.value),
							className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white font-mono focus:border-emerald-500"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 146,
							columnNumber: 15
						}, this)] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 142,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "sm:col-span-2",
							children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Tên đầy đủ của đơn vị*:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 156,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("input", {
								type: "text",
								required: true,
								placeholder: "VD: Ban Chỉ huy Quân sự Quận Ba Đình",
								value: name,
								onChange: (e) => setName(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white font-semibold focus:border-emerald-500"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 159,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 155,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 141,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-3 gap-3",
						children: [
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Tên viết tắt hiển thị bản đồ:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 173,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("input", {
								type: "text",
								placeholder: "VD: Ban CHQS Ba Đình",
								value: shortName,
								onChange: (e) => setShortName(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 176,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 172,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Cấp / Loại hình*:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 185,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: unitType,
								onChange: (e) => setUnitType(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500",
								children: [
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Ban Chỉ huy Quân sự",
										children: "Ban Chỉ huy Quân sự"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 193,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Cơ quan Bộ Quốc phòng",
										children: "Cơ quan Bộ Quốc phòng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 194,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Bộ Tư lệnh",
										children: "Bộ Tư lệnh"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 195,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Quân chủng",
										children: "Quân chủng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 196,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng",
										children: "Binh chủng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 197,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Sư đoàn",
										children: "Sư đoàn"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 198,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Lữ đoàn",
										children: "Lữ đoàn"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 199,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Trung đoàn",
										children: "Trung đoàn"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 200,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Tiểu đoàn",
										children: "Tiểu đoàn"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 201,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Học viện - Nhà trường",
										children: "Học viện - Nhà trường"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 202,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Bệnh viện Quân y",
										children: "Bệnh viện Quân y"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 203,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Kho - Trạm - Căn cứ",
										children: "Kho - Trạm - Căn cứ"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 204,
										columnNumber: 17
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 188,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 184,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Quân Binh chủng*:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 208,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: branch,
								onChange: (e) => setBranch(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500",
								children: [
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Lục quân",
										children: "Lục quân"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 216,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Phòng không - Không quân",
										children: "Phòng không - Không quân"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 217,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Tác chiến Không gian mạng",
										children: "Tác chiến Không gian mạng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 218,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng Đặc công",
										children: "Binh chủng Đặc công"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 219,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng Pháo binh",
										children: "Binh chủng Pháo binh"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 220,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng Tăng Thiết Giáp",
										children: "Binh chủng Tăng Thiết Giáp"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 221,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng Công binh",
										children: "Binh chủng Công binh"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 222,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng Thông tin liên lạc",
										children: "Binh chủng Thông tin liên lạc"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 223,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Binh chủng Hóa học",
										children: "Binh chủng Hóa học"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 224,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Bộ Tư lệnh Lăng",
										children: "Bộ Tư lệnh Lăng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 225,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Quân y",
										children: "Quân y"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 226,
										columnNumber: 17
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: "Tổng cục Hậu cần - Kỹ thuật",
										children: "Tổng cục Hậu cần - Kỹ thuật"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 227,
										columnNumber: 17
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 211,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 207,
								columnNumber: 13
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 171,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-2 gap-3",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
							className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
							children: "Trực thuộc đơn vị cấp trên:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 235,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("input", {
							type: "text",
							value: parentUnit,
							onChange: (e) => setParentUnit(e.target.value),
							className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 238,
							columnNumber: 15
						}, this)] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 234,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
							className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
							children: "Địa bàn Quận / Huyện Hà Nội*:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 246,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("select", {
							value: district,
							onChange: (e) => setDistrict(e.target.value),
							className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500",
							children: HANOI_DISTRICTS.map((d) => /* @__PURE__ */ _jsxDEV("option", {
								value: d.name,
								children: [
									d.name,
									" (",
									d.defensiveSector,
									")"
								]
							}, d.code, true, {
								fileName: _jsxFileName,
								lineNumber: 255,
								columnNumber: 19
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 249,
							columnNumber: 15
						}, this)] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 245,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 233,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
						className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
						children: "Địa chỉ đóng quân chi tiết*:"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 265,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("input", {
						type: "text",
						required: true,
						value: address,
						onChange: (e) => setAddress(e.target.value),
						className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 268,
						columnNumber: 13
					}, this)] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 264,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "p-3 bg-[#0c1425] border border-emerald-800/60 rounded-lg space-y-3",
						children: [/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-center justify-between text-emerald-400 font-bold text-[11px]",
							children: [/* @__PURE__ */ _jsxDEV("span", {
								className: "flex items-center gap-1.5",
								children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-4 h-4" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 281,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("span", { children: "TỌA ĐỘ KHÔNG GIAN GIS (TỰ ĐỘNG CHUYỂN ĐỔI HAI CHIỀU)" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 282,
									columnNumber: 17
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 280,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "font-mono text-[10px] text-slate-400",
								children: "WGS84 <=> VN-2000 (EPSG:32648)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 284,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 279,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "grid grid-cols-2 sm:grid-cols-4 gap-3 font-mono",
							children: [
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[9px] text-slate-400 uppercase block mb-1 font-sans",
									children: "Vĩ độ (Lat WGS84):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 291,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "number",
									step: "0.000001",
									required: true,
									value: lat,
									onChange: (e) => handleLatChange(Number(e.target.value)),
									className: "w-full bg-[#080d19] border border-slate-700 rounded p-1.5 text-sky-300 text-xs"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 294,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 290,
									columnNumber: 15
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[9px] text-slate-400 uppercase block mb-1 font-sans",
									children: "Kinh độ (Lng WGS84):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 304,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "number",
									step: "0.000001",
									required: true,
									value: lng,
									onChange: (e) => handleLngChange(Number(e.target.value)),
									className: "w-full bg-[#080d19] border border-slate-700 rounded p-1.5 text-sky-300 text-xs"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 307,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 303,
									columnNumber: 15
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[9px] text-slate-400 uppercase block mb-1 font-sans",
									children: "Tọa độ X Easting (m):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 317,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "number",
									step: "1",
									required: true,
									value: utmX,
									onChange: (e) => handleUtmXChange(Number(e.target.value)),
									className: "w-full bg-[#080d19] border border-slate-700 rounded p-1.5 text-emerald-300 text-xs"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 320,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 316,
									columnNumber: 15
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[9px] text-slate-400 uppercase block mb-1 font-sans",
									children: "Tọa độ Y Northing (m):"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 330,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "number",
									step: "1",
									required: true,
									value: utmY,
									onChange: (e) => handleUtmYChange(Number(e.target.value)),
									className: "w-full bg-[#080d19] border border-slate-700 rounded p-1.5 text-emerald-300 text-xs"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 333,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 329,
									columnNumber: 15
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 289,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 278,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-3 gap-3",
						children: [
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Chỉ huy trưởng:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 348,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("input", {
								type: "text",
								placeholder: "VD: Thượng tá Nguyễn Văn A",
								value: commander,
								onChange: (e) => setCommander(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 351,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 347,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Chính ủy / Chính trị viên:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 360,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("input", {
								type: "text",
								placeholder: "VD: Thượng tá Trần Văn B",
								value: politicalCommissar,
								onChange: (e) => setPoliticalCommissar(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 363,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 359,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Số máy trực ban tác chiến*:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 372,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("input", {
								type: "text",
								required: true,
								value: phone,
								onChange: (e) => setPhone(e.target.value),
								className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-emerald-300 font-mono focus:border-emerald-500"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 375,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 371,
								columnNumber: 13
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 346,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-2 gap-3",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
							className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
							children: "Trạng thái sẵn sàng chiến đấu (SSCĐ)*:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 388,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("select", {
							value: readinessStatus,
							onChange: (e) => setReadinessStatus(e.target.value),
							className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500",
							children: [
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Cấp 1 (Cao nhất)",
									children: "Cấp 1 (Cao nhất)"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 396,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Cấp 2 (Tăng cường)",
									children: "Cấp 2 (Tăng cường)"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 397,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Cấp 3 (Thường xuyên)",
									children: "Cấp 3 (Thường xuyên)"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 398,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Huấn luyện - Đào tạo",
									children: "Huấn luyện - Đào tạo"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 399,
									columnNumber: 17
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 391,
							columnNumber: 15
						}, this)] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 387,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
							className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
							children: "Cấp độ bảo mật CSDL*:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 403,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("select", {
							value: securityLevel,
							onChange: (e) => setSecurityLevel(e.target.value),
							className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500",
							children: [
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Mật",
									children: "Mật"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 411,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Tối mật",
									children: "Tối mật"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 412,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Tuyệt mật",
									children: "Tuyệt mật"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 413,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("option", {
									value: "Nội bộ",
									children: "Nội bộ"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 414,
									columnNumber: 17
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 406,
							columnNumber: 15
						}, this)] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 402,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 386,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
						className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
						children: "Nhiệm vụ tác chiến chính (phân cách bằng dấu chấm phẩy ;):"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 421,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("input", {
						type: "text",
						placeholder: "VD: Phòng thủ khu vực trung tâm; Huấn luyện SSCĐ; Cứu hộ lụt bão",
						value: missions,
						onChange: (e) => setMissions(e.target.value),
						className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 424,
						columnNumber: 13
					}, this)] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 420,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
						className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
						children: "Vũ khí, khí tài & trang thiết bị kỹ thuật (phân cách bằng dấu chấm phẩy ;):"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 434,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("input", {
						type: "text",
						placeholder: "VD: Xe chỉ huy cơ động; Súng máy phòng không 12.7mm; Thiết bị thông tin sóng cực ngắn",
						value: equipment,
						onChange: (e) => setEquipment(e.target.value),
						className: "w-full bg-[#0e1628] border border-slate-700 rounded p-2 text-white focus:border-emerald-500"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 437,
						columnNumber: 13
					}, this)] }, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 433,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "pt-4 border-t border-slate-800 flex justify-end gap-3",
						children: [/* @__PURE__ */ _jsxDEV("button", {
							type: "button",
							onClick: onClose,
							className: "px-4 py-2 rounded bg-slate-800 hover:bg-slate-700 text-slate-300 font-medium",
							children: "Hủy bỏ"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 448,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("button", {
							type: "submit",
							className: "px-5 py-2 rounded bg-emerald-600 hover:bg-emerald-500 text-white font-semibold flex items-center gap-1.5 shadow",
							children: [/* @__PURE__ */ _jsxDEV(Save, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 459,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Lưu CSDL Không Gian" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 460,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 455,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 447,
						columnNumber: 11
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 139,
				columnNumber: 9
			}, this)]
		}, void 0, true, {
			fileName: _jsxFileName,
			lineNumber: 124,
			columnNumber: 7
		}, this)
	}, void 0, false, {
		fileName: _jsxFileName,
		lineNumber: 123,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,eyJtYXBwaW5ncyI6IkFBQUEsT0FBTyxTQUFTLGdCQUEyQjtBQUUzQyxTQUFTLHVCQUF1QjtBQUNoQyxTQUFTLGVBQWUscUJBQXFCO0FBQzdDLFNBQVMsR0FBRyxNQUFNLFNBQXlCLGdCQUE4Qjs7O0FBU3pFLE9BQU8sTUFBTSxhQUF1QyxFQUNsRCxRQUNBLFNBQ0EsUUFDQSxrQkFDSTtDQUNKLElBQUksQ0FBQyxRQUFRLE9BQU87Q0FFcEIsTUFBTSxDQUFDLE1BQU0sV0FBVyxTQUFTLGFBQWEsUUFBUSxFQUFFO0NBQ3hELE1BQU0sQ0FBQyxNQUFNLFdBQVcsU0FBUyxhQUFhLFFBQVEsRUFBRTtDQUN4RCxNQUFNLENBQUMsV0FBVyxnQkFBZ0IsU0FBUyxhQUFhLGFBQWEsRUFBRTtDQUN2RSxNQUFNLENBQUMsVUFBVSxlQUFlLFNBQW1CLGFBQWEsWUFBWSxxQkFBcUI7Q0FDakcsTUFBTSxDQUFDLFFBQVEsYUFBYSxTQUF5QixhQUFhLFVBQVUsVUFBVTtDQUN0RixNQUFNLENBQUMsWUFBWSxpQkFBaUIsU0FBUyxhQUFhLGNBQWMsMEJBQTBCO0NBQ2xHLE1BQU0sQ0FBQyxVQUFVLGVBQWUsU0FBUyxhQUFhLFlBQVksY0FBYztDQUNoRixNQUFNLENBQUMsTUFBTSxXQUFXLFNBQVMsYUFBYSxRQUFRLEVBQUU7Q0FDeEQsTUFBTSxDQUFDLFNBQVMsY0FBYyxTQUFTLGFBQWEsV0FBVyxFQUFFOztDQUdqRSxNQUFNLENBQUMsS0FBSyxVQUFVLFNBQWlCLGFBQWEsWUFBWSxNQUFNO0NBQ3RFLE1BQU0sQ0FBQyxLQUFLLFVBQVUsU0FBaUIsYUFBYSxhQUFhLE1BQU07Q0FDdkUsTUFBTSxDQUFDLE1BQU0sV0FBVyxTQUFpQixhQUFhLFlBQVksS0FBTTtDQUN4RSxNQUFNLENBQUMsTUFBTSxXQUFXLFNBQWlCLGFBQWEsWUFBWSxNQUFPOztDQUd6RSxNQUFNLENBQUMsV0FBVyxnQkFBZ0IsU0FBUyxhQUFhLGFBQWEsRUFBRTtDQUN2RSxNQUFNLENBQUMsb0JBQW9CLHlCQUF5QixTQUFTLGFBQWEsc0JBQXNCLEVBQUU7Q0FDbEcsTUFBTSxDQUFDLE9BQU8sWUFBWSxTQUFTLGFBQWEsU0FBUyxNQUFNO0NBQy9ELE1BQU0sQ0FBQyxlQUFlLG9CQUFvQixTQUF3QixhQUFhLGlCQUFpQixLQUFLO0NBQ3JHLE1BQU0sQ0FBQyxpQkFBaUIsc0JBQXNCLFNBQzVDLGFBQWEsbUJBQW1CLGtCQUNsQztDQUNBLE1BQU0sQ0FBQyxnQkFBZ0IscUJBQXFCLFNBQVMsYUFBYSxrQkFBa0IsRUFBRTtDQUN0RixNQUFNLENBQUMsVUFBVSxlQUFlLFNBQVMsYUFBYSxhQUFhLEtBQUssSUFBSSxLQUFLLEVBQUU7Q0FDbkYsTUFBTSxDQUFDLFdBQVcsZ0JBQWdCLFNBQVMsYUFBYSxvQkFBb0IsS0FBSyxJQUFJLEtBQUssRUFBRTtDQUM1RixNQUFNLENBQUMsaUJBQWlCLHNCQUFzQixTQUFpQixhQUFhLG1CQUFtQixJQUFJO0NBQ25HLE1BQU0sQ0FBQyxPQUFPLFlBQVksU0FBUyxhQUFhLFNBQVMsRUFBRTs7Q0FHM0QsTUFBTSxtQkFBbUIsUUFBZ0I7RUFDdkMsT0FBTyxHQUFHO0VBQ1YsTUFBTSxZQUFZLGNBQWMsS0FBSyxHQUFHO0VBQ3hDLFFBQVEsVUFBVSxDQUFDO0VBQ25CLFFBQVEsVUFBVSxDQUFDO0NBQ3JCO0NBRUEsTUFBTSxtQkFBbUIsUUFBZ0I7RUFDdkMsT0FBTyxHQUFHO0VBQ1YsTUFBTSxZQUFZLGNBQWMsS0FBSyxHQUFHO0VBQ3hDLFFBQVEsVUFBVSxDQUFDO0VBQ25CLFFBQVEsVUFBVSxDQUFDO0NBQ3JCO0NBRUEsTUFBTSxvQkFBb0IsUUFBZ0I7RUFDeEMsUUFBUSxHQUFHO0VBQ1gsTUFBTSxZQUFZLGNBQWMsS0FBSyxJQUFJO0VBQ3pDLE9BQU8sVUFBVSxHQUFHO0VBQ3BCLE9BQU8sVUFBVSxHQUFHO0NBQ3RCO0NBRUEsTUFBTSxvQkFBb0IsUUFBZ0I7RUFDeEMsUUFBUSxHQUFHO0VBQ1gsTUFBTSxZQUFZLGNBQWMsTUFBTSxHQUFHO0VBQ3pDLE9BQU8sVUFBVSxHQUFHO0VBQ3BCLE9BQU8sVUFBVSxHQUFHO0NBQ3RCO0NBRUEsTUFBTSxnQkFBZ0IsTUFBdUI7RUFDM0MsRUFBRSxlQUFlO0VBRWpCLE1BQU0sT0FBcUI7R0FDekIsSUFBSSxhQUFhLE1BQU0sTUFBTSxLQUFLLElBQUk7R0FDdEMsTUFBTSxLQUFLLEtBQUs7R0FDaEIsTUFBTSxLQUFLLEtBQUs7R0FDaEIsV0FBVyxVQUFVLEtBQUssS0FBSyxLQUFLLEtBQUs7R0FDekM7R0FDQTtHQUNBLFlBQVksV0FBVyxLQUFLO0dBQzVCO0dBQ0EsTUFBTSxLQUFLLEtBQUs7R0FDaEIsU0FBUyxRQUFRLEtBQUs7R0FDdEIsVUFBVSxPQUFPLEdBQUc7R0FDcEIsV0FBVyxPQUFPLEdBQUc7R0FDckIsVUFBVSxPQUFPLElBQUk7R0FDckIsVUFBVSxPQUFPLElBQUk7R0FDckIsV0FBVyxVQUFVLEtBQUs7R0FDMUIsb0JBQW9CLG1CQUFtQixLQUFLO0dBQzVDLE9BQU8sTUFBTSxLQUFLO0dBQ2xCO0dBQ0E7R0FDQSxnQkFBZ0IsZUFBZSxLQUFLO0dBQ3BDLGNBQWMsU0FDWCxNQUFNLEdBQUcsQ0FBQyxDQUNWLEtBQUssTUFBTSxFQUFFLEtBQUssQ0FBQyxDQUFDLENBQ3BCLFFBQVEsTUFBTSxFQUFFLFNBQVMsQ0FBQztHQUM3QixxQkFBcUIsVUFDbEIsTUFBTSxHQUFHLENBQUMsQ0FDVixLQUFLLE1BQU0sRUFBRSxLQUFLLENBQUMsQ0FBQyxDQUNwQixRQUFRLE1BQU0sRUFBRSxTQUFTLENBQUM7R0FDN0IsaUJBQWlCLE9BQU8sZUFBZSxLQUFLO0dBQzVDLE9BQU8sTUFBTSxLQUFLO0dBQ2xCLFdBQVcsSUFBSSxLQUFLLENBQUMsQ0FBQyxZQUFZO0VBQ3BDO0VBRUEsT0FBTyxJQUFJO0VBQ1gsUUFBUTtDQUNWO0NBRUEsT0FDRSx3QkFBQyxPQUFEO0VBQUssV0FBVTtZQUNiLHdCQUFDLE9BQUQ7R0FBSyxXQUFVO2FBQWYsQ0FFRSx3QkFBQyxPQUFEO0lBQUssV0FBVTtjQUFmLENBQ0Usd0JBQUMsT0FBRDtLQUFLLFdBQVU7ZUFBZixDQUNFLHdCQUFDLFVBQUQsRUFBVSxXQUFVLDJCQUE0Qjs7OztlQUNoRCx3QkFBQyxNQUFEO01BQUksV0FBVTtnQkFDWCxjQUFjLHVDQUF1QztLQUNwRDs7OzthQUNEOzs7OztjQUNMLHdCQUFDLFVBQUQ7S0FBUSxTQUFTO0tBQVMsV0FBVTtlQUNsQyx3QkFBQyxHQUFELEVBQUcsV0FBVSxVQUFXOzs7OztJQUNsQjs7OztZQUNMOzs7OzthQUdMLHdCQUFDLFFBQUQ7SUFBTSxVQUFVO0lBQWMsV0FBVTtjQUF4QztLQUVFLHdCQUFDLE9BQUQ7TUFBSyxXQUFVO2dCQUFmLENBQ0Usd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFNBQUQ7T0FBTyxXQUFVO2lCQUE0RDtNQUV0RTs7OztnQkFDUCx3QkFBQyxTQUFEO09BQ0UsTUFBSztPQUNMO09BQ0EsYUFBWTtPQUNaLE9BQU87T0FDUCxXQUFXLE1BQU0sUUFBUSxFQUFFLE9BQU8sS0FBSztPQUN2QyxXQUFVO01BQ1g7Ozs7Y0FDRTs7OztnQkFDTCx3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBZixDQUNFLHdCQUFDLFNBQUQ7UUFBTyxXQUFVO2tCQUE0RDtPQUV0RTs7OztpQkFDUCx3QkFBQyxTQUFEO1FBQ0UsTUFBSztRQUNMO1FBQ0EsYUFBWTtRQUNaLE9BQU87UUFDUCxXQUFXLE1BQU0sUUFBUSxFQUFFLE9BQU8sS0FBSztRQUN2QyxXQUFVO09BQ1g7Ozs7ZUFDRTs7Ozs7Y0FDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWY7T0FDRSx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtRQUFPLFdBQVU7a0JBQTREO09BRXRFOzs7O2lCQUNQLHdCQUFDLFNBQUQ7UUFDRSxNQUFLO1FBQ0wsYUFBWTtRQUNaLE9BQU87UUFDUCxXQUFXLE1BQU0sYUFBYSxFQUFFLE9BQU8sS0FBSztRQUM1QyxXQUFVO09BQ1g7Ozs7ZUFDRTs7Ozs7T0FDTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtRQUFPLFdBQVU7a0JBQTREO09BRXRFOzs7O2lCQUNQLHdCQUFDLFVBQUQ7UUFDRSxPQUFPO1FBQ1AsV0FBVyxNQUFNLFlBQVksRUFBRSxPQUFPLEtBQWlCO1FBQ3ZELFdBQVU7a0JBSFo7U0FLRSx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBc0I7U0FBMkI7Ozs7O1NBQy9ELHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUF3QjtTQUE2Qjs7Ozs7U0FDbkUsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQWE7U0FBa0I7Ozs7O1NBQzdDLHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFhO1NBQWtCOzs7OztTQUM3Qyx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBYTtTQUFrQjs7Ozs7U0FDN0Msd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQVU7U0FBZTs7Ozs7U0FDdkMsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQVU7U0FBZTs7Ozs7U0FDdkMsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQWE7U0FBa0I7Ozs7O1NBQzdDLHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFZO1NBQWlCOzs7OztTQUMzQyx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBd0I7U0FBNkI7Ozs7O1NBQ25FLHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFtQjtTQUF3Qjs7Ozs7U0FDekQsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQXNCO1NBQTJCOzs7OztRQUN6RDs7Ozs7ZUFDTDs7Ozs7T0FDTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtRQUFPLFdBQVU7a0JBQTREO09BRXRFOzs7O2lCQUNQLHdCQUFDLFVBQUQ7UUFDRSxPQUFPO1FBQ1AsV0FBVyxNQUFNLFVBQVUsRUFBRSxPQUFPLEtBQXVCO1FBQzNELFdBQVU7a0JBSFo7U0FLRSx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBVztTQUFnQjs7Ozs7U0FDekMsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQTJCO1NBQWdDOzs7OztTQUN6RSx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBNEI7U0FBaUM7Ozs7O1NBQzNFLHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFzQjtTQUEyQjs7Ozs7U0FDL0Qsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQXVCO1NBQTRCOzs7OztTQUNqRSx3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBNkI7U0FBa0M7Ozs7O1NBQzdFLHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUF1QjtTQUE0Qjs7Ozs7U0FDakUsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQWdDO1NBQXFDOzs7OztTQUNuRix3QkFBQyxVQUFEO1VBQVEsT0FBTTtvQkFBcUI7U0FBMEI7Ozs7O1NBQzdELHdCQUFDLFVBQUQ7VUFBUSxPQUFNO29CQUFrQjtTQUF1Qjs7Ozs7U0FDdkQsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQVM7U0FBYzs7Ozs7U0FDckMsd0JBQUMsVUFBRDtVQUFRLE9BQU07b0JBQThCO1NBQW1DOzs7OztRQUN6RTs7Ozs7ZUFDTDs7Ozs7TUFDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtPQUFPLFdBQVU7aUJBQTREO01BRXRFOzs7O2dCQUNQLHdCQUFDLFNBQUQ7T0FDRSxNQUFLO09BQ0wsT0FBTztPQUNQLFdBQVcsTUFBTSxjQUFjLEVBQUUsT0FBTyxLQUFLO09BQzdDLFdBQVU7TUFDWDs7OztjQUNFOzs7O2dCQUNMLHdCQUFDLE9BQUQsYUFDRSx3QkFBQyxTQUFEO09BQU8sV0FBVTtpQkFBNEQ7TUFFdEU7Ozs7Z0JBQ1Asd0JBQUMsVUFBRDtPQUNFLE9BQU87T0FDUCxXQUFXLE1BQU0sWUFBWSxFQUFFLE9BQU8sS0FBSztPQUMzQyxXQUFVO2lCQUVULGdCQUFnQixLQUFLLE1BQ3BCLHdCQUFDLFVBQUQ7UUFBcUIsT0FBTyxFQUFFO2tCQUE5QjtTQUNHLEVBQUU7U0FBSztTQUFHLEVBQUU7U0FBZ0I7UUFDdkI7VUFGSyxFQUFFOzs7O2NBRVAsQ0FDVDtNQUNLOzs7O2NBQ0w7Ozs7Y0FDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFNBQUQ7TUFBTyxXQUFVO2dCQUE0RDtLQUV0RTs7OztlQUNQLHdCQUFDLFNBQUQ7TUFDRSxNQUFLO01BQ0w7TUFDQSxPQUFPO01BQ1AsV0FBVyxNQUFNLFdBQVcsRUFBRSxPQUFPLEtBQUs7TUFDMUMsV0FBVTtLQUNYOzs7O2FBQ0U7Ozs7O0tBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxPQUFEO09BQUssV0FBVTtpQkFBZixDQUNFLHdCQUFDLFFBQUQ7UUFBTSxXQUFVO2tCQUFoQixDQUNFLHdCQUFDLFNBQUQsRUFBUyxXQUFVLFVBQVc7Ozs7a0JBQzlCLHdCQUFDLFFBQUQsWUFBTSx1REFBMEQ7Ozs7Z0JBQzVEOzs7OztpQkFDTix3QkFBQyxRQUFEO1FBQU0sV0FBVTtrQkFBdUM7T0FFakQ7Ozs7ZUFDSDs7Ozs7Z0JBRUwsd0JBQUMsT0FBRDtPQUFLLFdBQVU7aUJBQWY7UUFDRSx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtTQUFPLFdBQVU7bUJBQTJEO1FBRXJFOzs7O2tCQUNQLHdCQUFDLFNBQUQ7U0FDRSxNQUFLO1NBQ0wsTUFBSztTQUNMO1NBQ0EsT0FBTztTQUNQLFdBQVcsTUFBTSxnQkFBZ0IsT0FBTyxFQUFFLE9BQU8sS0FBSyxDQUFDO1NBQ3ZELFdBQVU7UUFDWDs7OztnQkFDRTs7Ozs7UUFDTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtTQUFPLFdBQVU7bUJBQTJEO1FBRXJFOzs7O2tCQUNQLHdCQUFDLFNBQUQ7U0FDRSxNQUFLO1NBQ0wsTUFBSztTQUNMO1NBQ0EsT0FBTztTQUNQLFdBQVcsTUFBTSxnQkFBZ0IsT0FBTyxFQUFFLE9BQU8sS0FBSyxDQUFDO1NBQ3ZELFdBQVU7UUFDWDs7OztnQkFDRTs7Ozs7UUFDTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtTQUFPLFdBQVU7bUJBQTJEO1FBRXJFOzs7O2tCQUNQLHdCQUFDLFNBQUQ7U0FDRSxNQUFLO1NBQ0wsTUFBSztTQUNMO1NBQ0EsT0FBTztTQUNQLFdBQVcsTUFBTSxpQkFBaUIsT0FBTyxFQUFFLE9BQU8sS0FBSyxDQUFDO1NBQ3hELFdBQVU7UUFDWDs7OztnQkFDRTs7Ozs7UUFDTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtTQUFPLFdBQVU7bUJBQTJEO1FBRXJFOzs7O2tCQUNQLHdCQUFDLFNBQUQ7U0FDRSxNQUFLO1NBQ0wsTUFBSztTQUNMO1NBQ0EsT0FBTztTQUNQLFdBQVcsTUFBTSxpQkFBaUIsT0FBTyxFQUFFLE9BQU8sS0FBSyxDQUFDO1NBQ3hELFdBQVU7UUFDWDs7OztnQkFDRTs7Ozs7T0FDRjs7Ozs7Y0FDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWY7T0FDRSx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtRQUFPLFdBQVU7a0JBQTREO09BRXRFOzs7O2lCQUNQLHdCQUFDLFNBQUQ7UUFDRSxNQUFLO1FBQ0wsYUFBWTtRQUNaLE9BQU87UUFDUCxXQUFXLE1BQU0sYUFBYSxFQUFFLE9BQU8sS0FBSztRQUM1QyxXQUFVO09BQ1g7Ozs7ZUFDRTs7Ozs7T0FDTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtRQUFPLFdBQVU7a0JBQTREO09BRXRFOzs7O2lCQUNQLHdCQUFDLFNBQUQ7UUFDRSxNQUFLO1FBQ0wsYUFBWTtRQUNaLE9BQU87UUFDUCxXQUFXLE1BQU0sc0JBQXNCLEVBQUUsT0FBTyxLQUFLO1FBQ3JELFdBQVU7T0FDWDs7OztlQUNFOzs7OztPQUNMLHdCQUFDLE9BQUQsYUFDRSx3QkFBQyxTQUFEO1FBQU8sV0FBVTtrQkFBNEQ7T0FFdEU7Ozs7aUJBQ1Asd0JBQUMsU0FBRDtRQUNFLE1BQUs7UUFDTDtRQUNBLE9BQU87UUFDUCxXQUFXLE1BQU0sU0FBUyxFQUFFLE9BQU8sS0FBSztRQUN4QyxXQUFVO09BQ1g7Ozs7ZUFDRTs7Ozs7TUFDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRDtNQUFLLFdBQVU7Z0JBQWYsQ0FDRSx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtPQUFPLFdBQVU7aUJBQTREO01BRXRFOzs7O2dCQUNQLHdCQUFDLFVBQUQ7T0FDRSxPQUFPO09BQ1AsV0FBVyxNQUFNLG1CQUFtQixFQUFFLE9BQU8sS0FBd0I7T0FDckUsV0FBVTtpQkFIWjtRQUtFLHdCQUFDLFVBQUQ7U0FBUSxPQUFNO21CQUFtQjtRQUF3Qjs7Ozs7UUFDekQsd0JBQUMsVUFBRDtTQUFRLE9BQU07bUJBQXFCO1FBQTBCOzs7OztRQUM3RCx3QkFBQyxVQUFEO1NBQVEsT0FBTTttQkFBdUI7UUFBNEI7Ozs7O1FBQ2pFLHdCQUFDLFVBQUQ7U0FBUSxPQUFNO21CQUF1QjtRQUE0Qjs7Ozs7T0FDM0Q7Ozs7O2NBQ0w7Ozs7Z0JBQ0wsd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFNBQUQ7T0FBTyxXQUFVO2lCQUE0RDtNQUV0RTs7OztnQkFDUCx3QkFBQyxVQUFEO09BQ0UsT0FBTztPQUNQLFdBQVcsTUFBTSxpQkFBaUIsRUFBRSxPQUFPLEtBQXNCO09BQ2pFLFdBQVU7aUJBSFo7UUFLRSx3QkFBQyxVQUFEO1NBQVEsT0FBTTttQkFBTTtRQUFXOzs7OztRQUMvQix3QkFBQyxVQUFEO1NBQVEsT0FBTTttQkFBVTtRQUFlOzs7OztRQUN2Qyx3QkFBQyxVQUFEO1NBQVEsT0FBTTttQkFBWTtRQUFpQjs7Ozs7UUFDM0Msd0JBQUMsVUFBRDtTQUFRLE9BQU07bUJBQVM7UUFBYzs7Ozs7T0FDL0I7Ozs7O2NBQ0w7Ozs7Y0FDRjs7Ozs7O0tBR0wsd0JBQUMsT0FBRCxhQUNFLHdCQUFDLFNBQUQ7TUFBTyxXQUFVO2dCQUE0RDtLQUV0RTs7OztlQUNQLHdCQUFDLFNBQUQ7TUFDRSxNQUFLO01BQ0wsYUFBWTtNQUNaLE9BQU87TUFDUCxXQUFXLE1BQU0sWUFBWSxFQUFFLE9BQU8sS0FBSztNQUMzQyxXQUFVO0tBQ1g7Ozs7YUFDRTs7Ozs7S0FFTCx3QkFBQyxPQUFELGFBQ0Usd0JBQUMsU0FBRDtNQUFPLFdBQVU7Z0JBQTREO0tBRXRFOzs7O2VBQ1Asd0JBQUMsU0FBRDtNQUNFLE1BQUs7TUFDTCxhQUFZO01BQ1osT0FBTztNQUNQLFdBQVcsTUFBTSxhQUFhLEVBQUUsT0FBTyxLQUFLO01BQzVDLFdBQVU7S0FDWDs7OzthQUNFOzs7OztLQUdMLHdCQUFDLE9BQUQ7TUFBSyxXQUFVO2dCQUFmLENBQ0Usd0JBQUMsVUFBRDtPQUNFLE1BQUs7T0FDTCxTQUFTO09BQ1QsV0FBVTtpQkFDWDtNQUVPOzs7O2dCQUNSLHdCQUFDLFVBQUQ7T0FDRSxNQUFLO09BQ0wsV0FBVTtpQkFGWixDQUlFLHdCQUFDLE1BQUQsRUFBTSxXQUFVLFVBQVc7Ozs7aUJBQzNCLHdCQUFDLFFBQUQsWUFBTSxzQkFBeUI7Ozs7ZUFDekI7Ozs7O2NBQ0w7Ozs7OztJQUNEOzs7OztXQUNIOzs7Ozs7Q0FDRjs7Ozs7QUFFVCIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJVbml0TW9kYWwudHN4Il0sInZlcnNpb24iOjMsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBSZWFjdCwgeyB1c2VTdGF0ZSwgdXNlRWZmZWN0IH0gZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0LCBNaWxpdGFyeUJyYW5jaCwgVW5pdFR5cGUsIFJlYWRpbmVzc1N0YXR1cywgU2VjdXJpdHlMZXZlbCB9IGZyb20gJy4uL3R5cGVzL2dpcyc7XG5pbXBvcnQgeyBIQU5PSV9ESVNUUklDVFMgfSBmcm9tICcuLi9kYXRhL2hhbm9pRGlzdHJpY3RzJztcbmltcG9ydCB7IHdnczg0VG9VdG00OE4sIHV0bTQ4TlRvV2dzODQgfSBmcm9tICcuLi91dGlscy9jb29yZGluYXRlQ29udmVyc2lvbic7XG5pbXBvcnQgeyBYLCBTYXZlLCBDb21wYXNzLCBNYXBQaW4sIFNoaWVsZCwgQnVpbGRpbmcsIFBob25lLCBBd2FyZCB9IGZyb20gJ2x1Y2lkZS1yZWFjdCc7XG5cbmludGVyZmFjZSBVbml0TW9kYWxQcm9wcyB7XG4gIGlzT3BlbjogYm9vbGVhbjtcbiAgb25DbG9zZTogKCkgPT4gdm9pZDtcbiAgb25TYXZlOiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xuICBpbml0aWFsVW5pdD86IE1pbGl0YXJ5VW5pdCB8IG51bGw7XG59XG5cbmV4cG9ydCBjb25zdCBVbml0TW9kYWw6IFJlYWN0LkZDPFVuaXRNb2RhbFByb3BzPiA9ICh7XG4gIGlzT3BlbixcbiAgb25DbG9zZSxcbiAgb25TYXZlLFxuICBpbml0aWFsVW5pdCxcbn0pID0+IHtcbiAgaWYgKCFpc09wZW4pIHJldHVybiBudWxsO1xuXG4gIGNvbnN0IFtjb2RlLCBzZXRDb2RlXSA9IHVzZVN0YXRlKGluaXRpYWxVbml0Py5jb2RlIHx8ICcnKTtcbiAgY29uc3QgW25hbWUsIHNldE5hbWVdID0gdXNlU3RhdGUoaW5pdGlhbFVuaXQ/Lm5hbWUgfHwgJycpO1xuICBjb25zdCBbc2hvcnROYW1lLCBzZXRTaG9ydE5hbWVdID0gdXNlU3RhdGUoaW5pdGlhbFVuaXQ/LnNob3J0TmFtZSB8fCAnJyk7XG4gIGNvbnN0IFt1bml0VHlwZSwgc2V0VW5pdFR5cGVdID0gdXNlU3RhdGU8VW5pdFR5cGU+KGluaXRpYWxVbml0Py51bml0VHlwZSB8fCAnQmFuIENo4buJIGh1eSBRdcOibiBz4buxJyk7XG4gIGNvbnN0IFticmFuY2gsIHNldEJyYW5jaF0gPSB1c2VTdGF0ZTxNaWxpdGFyeUJyYW5jaD4oaW5pdGlhbFVuaXQ/LmJyYW5jaCB8fCAnTOG7pWMgcXXDom4nKTtcbiAgY29uc3QgW3BhcmVudFVuaXQsIHNldFBhcmVudFVuaXRdID0gdXNlU3RhdGUoaW5pdGlhbFVuaXQ/LnBhcmVudFVuaXQgfHwgJ0Lhu5kgVMawIGzhu4duaCBUaOG7pyDEkcO0IEjDoCBO4buZaScpO1xuICBjb25zdCBbZGlzdHJpY3QsIHNldERpc3RyaWN0XSA9IHVzZVN0YXRlKGluaXRpYWxVbml0Py5kaXN0cmljdCB8fCAnUXXhuq1uIEJhIMSQw6xuaCcpO1xuICBjb25zdCBbd2FyZCwgc2V0V2FyZF0gPSB1c2VTdGF0ZShpbml0aWFsVW5pdD8ud2FyZCB8fCAnJyk7XG4gIGNvbnN0IFthZGRyZXNzLCBzZXRBZGRyZXNzXSA9IHVzZVN0YXRlKGluaXRpYWxVbml0Py5hZGRyZXNzIHx8ICcnKTtcblxuICAvLyBDb29yZGluYXRlc1xuICBjb25zdCBbbGF0LCBzZXRMYXRdID0gdXNlU3RhdGU8bnVtYmVyPihpbml0aWFsVW5pdD8ubGF0aXR1ZGUgfHwgMjEuMDM0KTtcbiAgY29uc3QgW2xuZywgc2V0TG5nXSA9IHVzZVN0YXRlPG51bWJlcj4oaW5pdGlhbFVuaXQ/LmxvbmdpdHVkZSB8fCAxMDUuODMpO1xuICBjb25zdCBbdXRtWCwgc2V0VXRtWF0gPSB1c2VTdGF0ZTxudW1iZXI+KGluaXRpYWxVbml0Py54X3ZuMjAwMCB8fCA1ODYwMDApO1xuICBjb25zdCBbdXRtWSwgc2V0VXRtWV0gPSB1c2VTdGF0ZTxudW1iZXI+KGluaXRpYWxVbml0Py55X3ZuMjAwMCB8fCAyMzI2MDAwKTtcblxuICAvLyBDb21tYW5kICYgUmVhZGluZXNzXG4gIGNvbnN0IFtjb21tYW5kZXIsIHNldENvbW1hbmRlcl0gPSB1c2VTdGF0ZShpbml0aWFsVW5pdD8uY29tbWFuZGVyIHx8ICcnKTtcbiAgY29uc3QgW3BvbGl0aWNhbENvbW1pc3Nhciwgc2V0UG9saXRpY2FsQ29tbWlzc2FyXSA9IHVzZVN0YXRlKGluaXRpYWxVbml0Py5wb2xpdGljYWxDb21taXNzYXIgfHwgJycpO1xuICBjb25zdCBbcGhvbmUsIHNldFBob25lXSA9IHVzZVN0YXRlKGluaXRpYWxVbml0Py5waG9uZSB8fCAnMDI0LicpO1xuICBjb25zdCBbc2VjdXJpdHlMZXZlbCwgc2V0U2VjdXJpdHlMZXZlbF0gPSB1c2VTdGF0ZTxTZWN1cml0eUxldmVsPihpbml0aWFsVW5pdD8uc2VjdXJpdHlMZXZlbCB8fCAnTeG6rXQnKTtcbiAgY29uc3QgW3JlYWRpbmVzc1N0YXR1cywgc2V0UmVhZGluZXNzU3RhdHVzXSA9IHVzZVN0YXRlPFJlYWRpbmVzc1N0YXR1cz4oXG4gICAgaW5pdGlhbFVuaXQ/LnJlYWRpbmVzc1N0YXR1cyB8fCAnQ+G6pXAgMSAoQ2FvIG5o4bqldCknXG4gICk7XG4gIGNvbnN0IFtwZXJzb25uZWxTY2FsZSwgc2V0UGVyc29ubmVsU2NhbGVdID0gdXNlU3RhdGUoaW5pdGlhbFVuaXQ/LnBlcnNvbm5lbFNjYWxlIHx8ICcnKTtcbiAgY29uc3QgW21pc3Npb25zLCBzZXRNaXNzaW9uc10gPSB1c2VTdGF0ZShpbml0aWFsVW5pdD8ubWFpbk1pc3Npb25zLmpvaW4oJzsgJykgfHwgJycpO1xuICBjb25zdCBbZXF1aXBtZW50LCBzZXRFcXVpcG1lbnRdID0gdXNlU3RhdGUoaW5pdGlhbFVuaXQ/LmVxdWlwbWVudEhpZ2hsaWdodHMuam9pbignOyAnKSB8fCAnJyk7XG4gIGNvbnN0IFtlc3RhYmxpc2hlZFllYXIsIHNldEVzdGFibGlzaGVkWWVhcl0gPSB1c2VTdGF0ZTxudW1iZXI+KGluaXRpYWxVbml0Py5lc3RhYmxpc2hlZFllYXIgfHwgMTk1NCk7XG4gIGNvbnN0IFtub3Rlcywgc2V0Tm90ZXNdID0gdXNlU3RhdGUoaW5pdGlhbFVuaXQ/Lm5vdGVzIHx8ICcnKTtcblxuICAvLyBBdXRvIHN5bmMgV0dTODQgLT4gVVRNIDQ4TlxuICBjb25zdCBoYW5kbGVMYXRDaGFuZ2UgPSAodmFsOiBudW1iZXIpID0+IHtcbiAgICBzZXRMYXQodmFsKTtcbiAgICBjb25zdCBjb252ZXJ0ZWQgPSB3Z3M4NFRvVXRtNDhOKHZhbCwgbG5nKTtcbiAgICBzZXRVdG1YKGNvbnZlcnRlZC54KTtcbiAgICBzZXRVdG1ZKGNvbnZlcnRlZC55KTtcbiAgfTtcblxuICBjb25zdCBoYW5kbGVMbmdDaGFuZ2UgPSAodmFsOiBudW1iZXIpID0+IHtcbiAgICBzZXRMbmcodmFsKTtcbiAgICBjb25zdCBjb252ZXJ0ZWQgPSB3Z3M4NFRvVXRtNDhOKGxhdCwgdmFsKTtcbiAgICBzZXRVdG1YKGNvbnZlcnRlZC54KTtcbiAgICBzZXRVdG1ZKGNvbnZlcnRlZC55KTtcbiAgfTtcblxuICBjb25zdCBoYW5kbGVVdG1YQ2hhbmdlID0gKHZhbDogbnVtYmVyKSA9PiB7XG4gICAgc2V0VXRtWCh2YWwpO1xuICAgIGNvbnN0IGNvbnZlcnRlZCA9IHV0bTQ4TlRvV2dzODQodmFsLCB1dG1ZKTtcbiAgICBzZXRMYXQoY29udmVydGVkLmxhdCk7XG4gICAgc2V0TG5nKGNvbnZlcnRlZC5sbmcpO1xuICB9O1xuXG4gIGNvbnN0IGhhbmRsZVV0bVlDaGFuZ2UgPSAodmFsOiBudW1iZXIpID0+IHtcbiAgICBzZXRVdG1ZKHZhbCk7XG4gICAgY29uc3QgY29udmVydGVkID0gdXRtNDhOVG9XZ3M4NCh1dG1YLCB2YWwpO1xuICAgIHNldExhdChjb252ZXJ0ZWQubGF0KTtcbiAgICBzZXRMbmcoY29udmVydGVkLmxuZyk7XG4gIH07XG5cbiAgY29uc3QgaGFuZGxlU3VibWl0ID0gKGU6IFJlYWN0LkZvcm1FdmVudCkgPT4ge1xuICAgIGUucHJldmVudERlZmF1bHQoKTtcblxuICAgIGNvbnN0IHVuaXQ6IE1pbGl0YXJ5VW5pdCA9IHtcbiAgICAgIGlkOiBpbml0aWFsVW5pdD8uaWQgfHwgYERWXyR7RGF0ZS5ub3coKX1gLFxuICAgICAgY29kZTogY29kZS50cmltKCksXG4gICAgICBuYW1lOiBuYW1lLnRyaW0oKSxcbiAgICAgIHNob3J0TmFtZTogc2hvcnROYW1lLnRyaW0oKSB8fCBuYW1lLnRyaW0oKSxcbiAgICAgIHVuaXRUeXBlLFxuICAgICAgYnJhbmNoLFxuICAgICAgcGFyZW50VW5pdDogcGFyZW50VW5pdC50cmltKCksXG4gICAgICBkaXN0cmljdCxcbiAgICAgIHdhcmQ6IHdhcmQudHJpbSgpLFxuICAgICAgYWRkcmVzczogYWRkcmVzcy50cmltKCksXG4gICAgICBsYXRpdHVkZTogTnVtYmVyKGxhdCksXG4gICAgICBsb25naXR1ZGU6IE51bWJlcihsbmcpLFxuICAgICAgeF92bjIwMDA6IE51bWJlcih1dG1YKSxcbiAgICAgIHlfdm4yMDAwOiBOdW1iZXIodXRtWSksXG4gICAgICBjb21tYW5kZXI6IGNvbW1hbmRlci50cmltKCksXG4gICAgICBwb2xpdGljYWxDb21taXNzYXI6IHBvbGl0aWNhbENvbW1pc3Nhci50cmltKCksXG4gICAgICBwaG9uZTogcGhvbmUudHJpbSgpLFxuICAgICAgc2VjdXJpdHlMZXZlbCxcbiAgICAgIHJlYWRpbmVzc1N0YXR1cyxcbiAgICAgIHBlcnNvbm5lbFNjYWxlOiBwZXJzb25uZWxTY2FsZS50cmltKCksXG4gICAgICBtYWluTWlzc2lvbnM6IG1pc3Npb25zXG4gICAgICAgIC5zcGxpdCgnOycpXG4gICAgICAgIC5tYXAoKG0pID0+IG0udHJpbSgpKVxuICAgICAgICAuZmlsdGVyKChtKSA9PiBtLmxlbmd0aCA+IDApLFxuICAgICAgZXF1aXBtZW50SGlnaGxpZ2h0czogZXF1aXBtZW50XG4gICAgICAgIC5zcGxpdCgnOycpXG4gICAgICAgIC5tYXAoKGUpID0+IGUudHJpbSgpKVxuICAgICAgICAuZmlsdGVyKChlKSA9PiBlLmxlbmd0aCA+IDApLFxuICAgICAgZXN0YWJsaXNoZWRZZWFyOiBOdW1iZXIoZXN0YWJsaXNoZWRZZWFyKSB8fCAxOTU0LFxuICAgICAgbm90ZXM6IG5vdGVzLnRyaW0oKSxcbiAgICAgIHVwZGF0ZWRBdDogbmV3IERhdGUoKS50b0lTT1N0cmluZygpLFxuICAgIH07XG5cbiAgICBvblNhdmUodW5pdCk7XG4gICAgb25DbG9zZSgpO1xuICB9O1xuXG4gIHJldHVybiAoXG4gICAgPGRpdiBjbGFzc05hbWU9XCJmaXhlZCBpbnNldC0wIHotWzIwMDBdIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIGJnLWJsYWNrLzc1IGJhY2tkcm9wLWJsdXItc20gcC00IG92ZXJmbG93LXktYXV0b1wiPlxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJyZWxhdGl2ZSB3LWZ1bGwgbWF4LXctM3hsIGJnLVsjMDkxMDFmXSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkLXhsIHNoYWRvdy0yeGwgdGV4dC13aGl0ZSBvdmVyZmxvdy1oaWRkZW4gbXktNlwiPlxuICAgICAgICB7LyogTW9kYWwgSGVhZGVyICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInAtNCBiZy1ncmFkaWVudC10by1yIGZyb20tc2xhdGUtOTAwIHRvLXNsYXRlLTgwMCBib3JkZXItYiBib3JkZXItc2xhdGUtNzAwIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlblwiPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgIDxCdWlsZGluZyBjbGFzc05hbWU9XCJ3LTUgaC01IHRleHQtZW1lcmFsZC00MDBcIiAvPlxuICAgICAgICAgICAgPGgzIGNsYXNzTmFtZT1cInRleHQtYmFzZSBmb250LWJvbGQgdGV4dC13aGl0ZVwiPlxuICAgICAgICAgICAgICB7aW5pdGlhbFVuaXQgPyAnQ2jhu4luaCBz4butYSBUaMO0bmcgdGluIMSQxqFuIHbhu4sgUXXDom4gc+G7sScgOiAnVGjDqm0gTeG7m2kgxJDGoW4gduG7iyBRdcOibiDEkeG7mWkgdHLDqm4gSMOgIE7hu5lpJ31cbiAgICAgICAgICAgIDwvaDM+XG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPGJ1dHRvbiBvbkNsaWNrPXtvbkNsb3NlfSBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBob3Zlcjp0ZXh0LXdoaXRlIHAtMVwiPlxuICAgICAgICAgICAgPFggY2xhc3NOYW1lPVwidy01IGgtNVwiIC8+XG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHsvKiBGb3JtIEJvZHkgKi99XG4gICAgICAgIDxmb3JtIG9uU3VibWl0PXtoYW5kbGVTdWJtaXR9IGNsYXNzTmFtZT1cInAtNiBzcGFjZS15LTQgbWF4LWgtWzgwdmhdIG92ZXJmbG93LXktYXV0byB0ZXh0LXhzXCI+XG4gICAgICAgICAgey8qIFJvdyAxOiBDb2RlICYgTmFtZSAqL31cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTEgc206Z3JpZC1jb2xzLTMgZ2FwLTNcIj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICBNw6MgxJHGoW4gduG7iyAoQ29kZSkqOlxuICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgICB0eXBlPVwidGV4dFwiXG4gICAgICAgICAgICAgICAgcmVxdWlyZWRcbiAgICAgICAgICAgICAgICBwbGFjZWhvbGRlcj1cIlZEOiBCQ0gtQkQsIFNELTM2MVwiXG4gICAgICAgICAgICAgICAgdmFsdWU9e2NvZGV9XG4gICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRDb2RlKGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtd2hpdGUgZm9udC1tb25vIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic206Y29sLXNwYW4tMlwiPlxuICAgICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgICAgVMOqbiDEkeG6p3kgxJHhu6cgY+G7p2EgxJHGoW4gduG7iyo6XG4gICAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICAgIHR5cGU9XCJ0ZXh0XCJcbiAgICAgICAgICAgICAgICByZXF1aXJlZFxuICAgICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiVkQ6IEJhbiBDaOG7iSBodXkgUXXDom4gc+G7sSBRdeG6rW4gQmEgxJDDrG5oXCJcbiAgICAgICAgICAgICAgICB2YWx1ZT17bmFtZX1cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldE5hbWUoZS50YXJnZXQudmFsdWUpfVxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTIgdGV4dC13aGl0ZSBmb250LXNlbWlib2xkIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBSb3cgMjogU2hvcnQgbmFtZSAmIFVuaXQgVHlwZSAmIEJyYW5jaCAqL31cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTEgc206Z3JpZC1jb2xzLTMgZ2FwLTNcIj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICBUw6puIHZp4bq/dCB04bqvdCBoaeG7g24gdGjhu4sgYuG6o24gxJHhu5M6XG4gICAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICAgIHR5cGU9XCJ0ZXh0XCJcbiAgICAgICAgICAgICAgICBwbGFjZWhvbGRlcj1cIlZEOiBCYW4gQ0hRUyBCYSDEkMOsbmhcIlxuICAgICAgICAgICAgICAgIHZhbHVlPXtzaG9ydE5hbWV9XG4gICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRTaG9ydE5hbWUoZS50YXJnZXQudmFsdWUpfVxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTIgdGV4dC13aGl0ZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDBcIlxuICAgICAgICAgICAgICAvPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgICAgQ+G6pXAgLyBMb+G6oWkgaMOsbmgqOlxuICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgICAgdmFsdWU9e3VuaXRUeXBlfVxuICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0VW5pdFR5cGUoZS50YXJnZXQudmFsdWUgYXMgVW5pdFR5cGUpfVxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTIgdGV4dC13aGl0ZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDBcIlxuICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkJhbiBDaOG7iSBodXkgUXXDom4gc+G7sVwiPkJhbiBDaOG7iSBodXkgUXXDom4gc+G7sTwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJDxqEgcXVhbiBC4buZIFF14buRYyBwaMOybmdcIj5DxqEgcXVhbiBC4buZIFF14buRYyBwaMOybmc8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiQuG7mSBUxrAgbOG7h25oXCI+QuG7mSBUxrAgbOG7h25oPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlF1w6JuIGNo4bunbmdcIj5RdcOibiBjaOG7p25nPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkJpbmggY2jhu6duZ1wiPkJpbmggY2jhu6duZzwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJTxrAgxJFvw6BuXCI+U8awIMSRb8Ogbjwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJM4buvIMSRb8OgblwiPkzhu68gxJFvw6BuPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlRydW5nIMSRb8OgblwiPlRydW5nIMSRb8Ogbjwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJUaeG7g3UgxJFvw6BuXCI+VGnhu4N1IMSRb8Ogbjwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJI4buNYyB2aeG7h24gLSBOaMOgIHRyxrDhu51uZ1wiPkjhu41jIHZp4buHbiAtIE5ow6AgdHLGsOG7nW5nPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkLhu4duaCB2aeG7h24gUXXDom4geVwiPkLhu4duaCB2aeG7h24gUXXDom4geTwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJLaG8gLSBUcuG6oW0gLSBDxINuIGPhu6lcIj5LaG8gLSBUcuG6oW0gLSBDxINuIGPhu6k8L29wdGlvbj5cbiAgICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICBRdcOibiBCaW5oIGNo4bunbmcqOlxuICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgICAgdmFsdWU9e2JyYW5jaH1cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldEJyYW5jaChlLnRhcmdldC52YWx1ZSBhcyBNaWxpdGFyeUJyYW5jaCl9XG4gICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidy1mdWxsIGJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkIHAtMiB0ZXh0LXdoaXRlIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiTOG7pWMgcXXDom5cIj5M4bulYyBxdcOibjwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJQaMOybmcga2jDtG5nIC0gS2jDtG5nIHF1w6JuXCI+UGjDsm5nIGtow7RuZyAtIEtow7RuZyBxdcOibjwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJUw6FjIGNoaeG6v24gS2jDtG5nIGdpYW4gbeG6oW5nXCI+VMOhYyBjaGnhur9uIEtow7RuZyBnaWFuIG3huqFuZzwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJCaW5oIGNo4bunbmcgxJDhurdjIGPDtG5nXCI+QmluaCBjaOG7p25nIMSQ4bq3YyBjw7RuZzwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJCaW5oIGNo4bunbmcgUGjDoW8gYmluaFwiPkJpbmggY2jhu6duZyBQaMOhbyBiaW5oPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkJpbmggY2jhu6duZyBUxINuZyBUaGnhur90IEdpw6FwXCI+QmluaCBjaOG7p25nIFTEg25nIFRoaeG6v3QgR2nDoXA8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiQmluaCBjaOG7p25nIEPDtG5nIGJpbmhcIj5CaW5oIGNo4bunbmcgQ8O0bmcgYmluaDwvb3B0aW9uPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJCaW5oIGNo4bunbmcgVGjDtG5nIHRpbiBsacOqbiBs4bqhY1wiPkJpbmggY2jhu6duZyBUaMO0bmcgdGluIGxpw6puIGzhuqFjPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkJpbmggY2jhu6duZyBIw7NhIGjhu41jXCI+QmluaCBjaOG7p25nIEjDs2EgaOG7jWM8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiQuG7mSBUxrAgbOG7h25oIEzEg25nXCI+QuG7mSBUxrAgbOG7h25oIEzEg25nPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlF1w6JuIHlcIj5RdcOibiB5PC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlThu5VuZyBj4bulYyBI4bqtdSBj4bqnbiAtIEvhu7kgdGh14bqtdFwiPlThu5VuZyBj4bulYyBI4bqtdSBj4bqnbiAtIEvhu7kgdGh14bqtdDwvb3B0aW9uPlxuICAgICAgICAgICAgICA8L3NlbGVjdD5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIFJvdyAzOiBQYXJlbnQgJiBEaXN0cmljdCAqL31cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTEgc206Z3JpZC1jb2xzLTIgZ2FwLTNcIj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICBUcuG7sWMgdGh14buZYyDEkcahbiB24buLIGPhuqVwIHRyw6puOlxuICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgICB0eXBlPVwidGV4dFwiXG4gICAgICAgICAgICAgICAgdmFsdWU9e3BhcmVudFVuaXR9XG4gICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRQYXJlbnRVbml0KGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtd2hpdGUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwXCJcbiAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSBmb250LWJvbGQgYmxvY2sgbWItMVwiPlxuICAgICAgICAgICAgICAgIMSQ4buLYSBiw6BuIFF14bqtbiAvIEh1eeG7h24gSMOgIE7hu5lpKjpcbiAgICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgICAgPHNlbGVjdFxuICAgICAgICAgICAgICAgIHZhbHVlPXtkaXN0cmljdH1cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldERpc3RyaWN0KGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtd2hpdGUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwXCJcbiAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgIHtIQU5PSV9ESVNUUklDVFMubWFwKChkKSA9PiAoXG4gICAgICAgICAgICAgICAgICA8b3B0aW9uIGtleT17ZC5jb2RlfSB2YWx1ZT17ZC5uYW1lfT5cbiAgICAgICAgICAgICAgICAgICAge2QubmFtZX0gKHtkLmRlZmVuc2l2ZVNlY3Rvcn0pXG4gICAgICAgICAgICAgICAgICA8L29wdGlvbj5cbiAgICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBEZXRhaWxlZCBBZGRyZXNzICovfVxuICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgIMSQ4buLYSBjaOG7iSDEkcOzbmcgcXXDom4gY2hpIHRp4bq/dCo6XG4gICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgIHR5cGU9XCJ0ZXh0XCJcbiAgICAgICAgICAgICAgcmVxdWlyZWRcbiAgICAgICAgICAgICAgdmFsdWU9e2FkZHJlc3N9XG4gICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0QWRkcmVzcyhlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTIgdGV4dC13aGl0ZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDBcIlxuICAgICAgICAgICAgLz5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBTcGF0aWFsIENvb3JkaW5hdGVzIEJveCB3aXRoIFJlYWwtdGltZSBDb252ZXJzaW9uICovfVxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC0zIGJnLVsjMGMxNDI1XSBib3JkZXIgYm9yZGVyLWVtZXJhbGQtODAwLzYwIHJvdW5kZWQtbGcgc3BhY2UteS0zXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiB0ZXh0LWVtZXJhbGQtNDAwIGZvbnQtYm9sZCB0ZXh0LVsxMXB4XVwiPlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCI+XG4gICAgICAgICAgICAgICAgPENvbXBhc3MgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICAgICAgPHNwYW4+VOG7jEEgxJDhu5ggS0jDlE5HIEdJQU4gR0lTIChU4buwIMSQ4buYTkcgQ0hVWeG7gk4gxJDhu5RJIEhBSSBDSEnhu4BVKTwvc3Bhbj5cbiAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDBcIj5cbiAgICAgICAgICAgICAgICBXR1M4NCAmbHQ7PSZndDsgVk4tMjAwMCAoRVBTRzozMjY0OClcbiAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMiBzbTpncmlkLWNvbHMtNCBnYXAtMyBmb250LW1vbm9cIj5cbiAgICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bOXB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgYmxvY2sgbWItMSBmb250LXNhbnNcIj5cbiAgICAgICAgICAgICAgICAgIFbEqSDEkeG7mSAoTGF0IFdHUzg0KTpcbiAgICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICAgICAgdHlwZT1cIm51bWJlclwiXG4gICAgICAgICAgICAgICAgICBzdGVwPVwiMC4wMDAwMDFcIlxuICAgICAgICAgICAgICAgICAgcmVxdWlyZWRcbiAgICAgICAgICAgICAgICAgIHZhbHVlPXtsYXR9XG4gICAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IGhhbmRsZUxhdENoYW5nZShOdW1iZXIoZS50YXJnZXQudmFsdWUpKX1cbiAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzA4MGQxOV0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTEuNSB0ZXh0LXNreS0zMDAgdGV4dC14c1wiXG4gICAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzlweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGJsb2NrIG1iLTEgZm9udC1zYW5zXCI+XG4gICAgICAgICAgICAgICAgICBLaW5oIMSR4buZIChMbmcgV0dTODQpOlxuICAgICAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgICB0eXBlPVwibnVtYmVyXCJcbiAgICAgICAgICAgICAgICAgIHN0ZXA9XCIwLjAwMDAwMVwiXG4gICAgICAgICAgICAgICAgICByZXF1aXJlZFxuICAgICAgICAgICAgICAgICAgdmFsdWU9e2xuZ31cbiAgICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gaGFuZGxlTG5nQ2hhbmdlKE51bWJlcihlLnRhcmdldC52YWx1ZSkpfVxuICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidy1mdWxsIGJnLVsjMDgwZDE5XSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkIHAtMS41IHRleHQtc2t5LTMwMCB0ZXh0LXhzXCJcbiAgICAgICAgICAgICAgICAvPlxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bOXB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgYmxvY2sgbWItMSBmb250LXNhbnNcIj5cbiAgICAgICAgICAgICAgICAgIFThu41hIMSR4buZIFggRWFzdGluZyAobSk6XG4gICAgICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgICAgIHR5cGU9XCJudW1iZXJcIlxuICAgICAgICAgICAgICAgICAgc3RlcD1cIjFcIlxuICAgICAgICAgICAgICAgICAgcmVxdWlyZWRcbiAgICAgICAgICAgICAgICAgIHZhbHVlPXt1dG1YfVxuICAgICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBoYW5kbGVVdG1YQ2hhbmdlKE51bWJlcihlLnRhcmdldC52YWx1ZSkpfVxuICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidy1mdWxsIGJnLVsjMDgwZDE5XSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkIHAtMS41IHRleHQtZW1lcmFsZC0zMDAgdGV4dC14c1wiXG4gICAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzlweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGJsb2NrIG1iLTEgZm9udC1zYW5zXCI+XG4gICAgICAgICAgICAgICAgICBU4buNYSDEkeG7mSBZIE5vcnRoaW5nIChtKTpcbiAgICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICAgICAgdHlwZT1cIm51bWJlclwiXG4gICAgICAgICAgICAgICAgICBzdGVwPVwiMVwiXG4gICAgICAgICAgICAgICAgICByZXF1aXJlZFxuICAgICAgICAgICAgICAgICAgdmFsdWU9e3V0bVl9XG4gICAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IGhhbmRsZVV0bVlDaGFuZ2UoTnVtYmVyKGUudGFyZ2V0LnZhbHVlKSl9XG4gICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwODBkMTldIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0xLjUgdGV4dC1lbWVyYWxkLTMwMCB0ZXh0LXhzXCJcbiAgICAgICAgICAgICAgICAvPlxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIENvbW1hbmRlciAmIENvbnRhY3QgKi99XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIHNtOmdyaWQtY29scy0zIGdhcC0zXCI+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgICAgQ2jhu4kgaHV5IHRyxrDhu59uZzpcbiAgICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgdHlwZT1cInRleHRcIlxuICAgICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiVkQ6IFRoxrDhu6NuZyB0w6EgTmd1eeG7hW4gVsSDbiBBXCJcbiAgICAgICAgICAgICAgICB2YWx1ZT17Y29tbWFuZGVyfVxuICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0Q29tbWFuZGVyKGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtd2hpdGUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwXCJcbiAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSBmb250LWJvbGQgYmxvY2sgbWItMVwiPlxuICAgICAgICAgICAgICAgIENow61uaCDhu6d5IC8gQ2jDrW5oIHRy4buLIHZpw6puOlxuICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgICB0eXBlPVwidGV4dFwiXG4gICAgICAgICAgICAgICAgcGxhY2Vob2xkZXI9XCJWRDogVGjGsOG7o25nIHTDoSBUcuG6p24gVsSDbiBCXCJcbiAgICAgICAgICAgICAgICB2YWx1ZT17cG9saXRpY2FsQ29tbWlzc2FyfVxuICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0UG9saXRpY2FsQ29tbWlzc2FyKGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtd2hpdGUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwXCJcbiAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSBmb250LWJvbGQgYmxvY2sgbWItMVwiPlxuICAgICAgICAgICAgICAgIFPhu5EgbcOheSB0cuG7sWMgYmFuIHTDoWMgY2hp4bq/bio6XG4gICAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICAgIHR5cGU9XCJ0ZXh0XCJcbiAgICAgICAgICAgICAgICByZXF1aXJlZFxuICAgICAgICAgICAgICAgIHZhbHVlPXtwaG9uZX1cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFBob25lKGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtZW1lcmFsZC0zMDAgZm9udC1tb25vIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBSZWFkaW5lc3MgJiBTZWN1cml0eSAqL31cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTEgc206Z3JpZC1jb2xzLTIgZ2FwLTNcIj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICBUcuG6oW5nIHRow6FpIHPhurVuIHPDoG5nIGNoaeG6v24gxJHhuqV1IChTU0PEkCkqOlxuICAgICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgICAgdmFsdWU9e3JlYWRpbmVzc1N0YXR1c31cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFJlYWRpbmVzc1N0YXR1cyhlLnRhcmdldC52YWx1ZSBhcyBSZWFkaW5lc3NTdGF0dXMpfVxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTIgdGV4dC13aGl0ZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDBcIlxuICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkPhuqVwIDEgKENhbyBuaOG6pXQpXCI+Q+G6pXAgMSAoQ2FvIG5o4bqldCk8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiQ+G6pXAgMiAoVMSDbmcgY8aw4budbmcpXCI+Q+G6pXAgMiAoVMSDbmcgY8aw4budbmcpPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIkPhuqVwIDMgKFRoxrDhu51uZyB4dXnDqm4pXCI+Q+G6pXAgMyAoVGjGsOG7nW5nIHh1ecOqbik8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiSHXhuqVuIGx1eeG7h24gLSDEkMOgbyB04bqhb1wiPkh14bqlbiBsdXnhu4duIC0gxJDDoG8gdOG6oW88L29wdGlvbj5cbiAgICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICBD4bqlcCDEkeG7mSBi4bqjbyBt4bqtdCBDU0RMKjpcbiAgICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgICAgPHNlbGVjdFxuICAgICAgICAgICAgICAgIHZhbHVlPXtzZWN1cml0eUxldmVsfVxuICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0U2VjdXJpdHlMZXZlbChlLnRhcmdldC52YWx1ZSBhcyBTZWN1cml0eUxldmVsKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQgcC0yIHRleHQtd2hpdGUgZm9jdXM6Ym9yZGVyLWVtZXJhbGQtNTAwXCJcbiAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9XCJN4bqtdFwiPk3huq10PC9vcHRpb24+XG4gICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlThu5FpIG3huq10XCI+VOG7kWkgbeG6rXQ8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiVHV54buHdCBt4bqtdFwiPlR1eeG7h3QgbeG6rXQ8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiTuG7mWkgYuG7mVwiPk7hu5lpIGLhu5k8L29wdGlvbj5cbiAgICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBNaXNzaW9ucyBhbmQgRXF1aXBtZW50cyAoc2VtaWNvbG9uIHNlcGFyYXRlZCkgKi99XG4gICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgTmhp4buHbSB24bulIHTDoWMgY2hp4bq/biBjaMOtbmggKHBow6JuIGPDoWNoIGLhurFuZyBk4bqldSBjaOG6pW0gcGjhuql5IDspOlxuICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICB0eXBlPVwidGV4dFwiXG4gICAgICAgICAgICAgIHBsYWNlaG9sZGVyPVwiVkQ6IFBow7JuZyB0aOG7pyBraHUgduG7sWMgdHJ1bmcgdMOibTsgSHXhuqVuIGx1eeG7h24gU1NDxJA7IEPhu6l1IGjhu5kgbOG7pXQgYsOjb1wiXG4gICAgICAgICAgICAgIHZhbHVlPXttaXNzaW9uc31cbiAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRNaXNzaW9ucyhlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZCBwLTIgdGV4dC13aGl0ZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDBcIlxuICAgICAgICAgICAgLz5cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgIFbFqSBraMOtLCBraMOtIHTDoGkgJiB0cmFuZyB0aGnhur90IGLhu4sga+G7uSB0aHXhuq10IChwaMOibiBjw6FjaCBi4bqxbmcgZOG6pXUgY2jhuqVtIHBo4bqpeSA7KTpcbiAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgdHlwZT1cInRleHRcIlxuICAgICAgICAgICAgICBwbGFjZWhvbGRlcj1cIlZEOiBYZSBjaOG7iSBodXkgY8ahIMSR4buZbmc7IFPDum5nIG3DoXkgcGjDsm5nIGtow7RuZyAxMi43bW07IFRoaeG6v3QgYuG7iyB0aMO0bmcgdGluIHPDs25nIGPhu7FjIG5n4bqvblwiXG4gICAgICAgICAgICAgIHZhbHVlPXtlcXVpcG1lbnR9XG4gICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0RXF1aXBtZW50KGUudGFyZ2V0LnZhbHVlKX1cbiAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidy1mdWxsIGJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkIHAtMiB0ZXh0LXdoaXRlIGZvY3VzOmJvcmRlci1lbWVyYWxkLTUwMFwiXG4gICAgICAgICAgICAvPlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIEFjdGlvbiBidXR0b25zICovfVxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicHQtNCBib3JkZXItdCBib3JkZXItc2xhdGUtODAwIGZsZXgganVzdGlmeS1lbmQgZ2FwLTNcIj5cbiAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgdHlwZT1cImJ1dHRvblwiXG4gICAgICAgICAgICAgIG9uQ2xpY2s9e29uQ2xvc2V9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInB4LTQgcHktMiByb3VuZGVkIGJnLXNsYXRlLTgwMCBob3ZlcjpiZy1zbGF0ZS03MDAgdGV4dC1zbGF0ZS0zMDAgZm9udC1tZWRpdW1cIlxuICAgICAgICAgICAgPlxuICAgICAgICAgICAgICBI4buneSBi4buPXG4gICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgdHlwZT1cInN1Ym1pdFwiXG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInB4LTUgcHktMiByb3VuZGVkIGJnLWVtZXJhbGQtNjAwIGhvdmVyOmJnLWVtZXJhbGQtNTAwIHRleHQtd2hpdGUgZm9udC1zZW1pYm9sZCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41IHNoYWRvd1wiXG4gICAgICAgICAgICA+XG4gICAgICAgICAgICAgIDxTYXZlIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICAgICAgICA8c3Bhbj5MxrB1IENTREwgS2jDtG5nIEdpYW48L3NwYW4+XG4gICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9mb3JtPlxuICAgICAgPC9kaXY+XG4gICAgPC9kaXY+XG4gICk7XG59O1xuIl19