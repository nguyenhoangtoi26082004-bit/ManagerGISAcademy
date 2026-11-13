const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"];const _jsxDEV = __vite__cjsImport3_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { generatePostgisSQL } from "/src/utils/postgisExport.ts";
import { Database, X, Copy, Check, Download, Terminal, Server, Layers } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/PostgisModal.tsx";
import __vite__cjsImport3_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const PostgisModal = ({ isOpen, onClose, units }) => {
	if (!isOpen) return null;
	const [copied, setCopied] = useState(false);
	const [activeTab, setActiveTab] = useState("info");
	const [selectedDemoQuery, setSelectedDemoQuery] = useState(1);
	const fullSQL = generatePostgisSQL(units);
	const handleCopy = () => {
		navigator.clipboard.writeText(fullSQL);
		setCopied(true);
		setTimeout(() => setCopied(false), 2e3);
	};
	const handleDownload = () => {
		const blob = new Blob([fullSQL], { type: "text/sql" });
		const url = URL.createObjectURL(blob);
		const a = document.createElement("a");
		a.href = url;
		a.download = "hanoi_military_gis_postgis.sql";
		document.body.appendChild(a);
		a.click();
		document.body.removeChild(a);
		URL.revokeObjectURL(url);
	};
	// Demo queries
	const demoQueries = [
		{
			id: 1,
			title: "ST_DWithin: Tìm đơn vị trong bán kính 10 km quanh Sở Chỉ huy BQP",
			sql: `SELECT ma_don_vi, ten_don_vi, quan_binh_chung,
       ROUND(ST_Distance(geom_32648, (SELECT geom_32648 FROM don_vi_quan_doi WHERE ma_don_vi = 'BQP-HQ'))::numeric, 2) AS khoang_cach_m
FROM don_vi_quan_doi
WHERE ST_DWithin(geom_32648, (SELECT geom_32648 FROM don_vi_quan_doi WHERE ma_don_vi = 'BQP-HQ'), 10000)
ORDER BY khoang_cach_m ASC;`,
			resultSample: [
				{
					ma_don_vi: "BQP-HQ",
					ten_don_vi: "Sở Chỉ huy Bộ Quốc phòng",
					culy: "0 m"
				},
				{
					ma_don_vi: "BTL-LANG",
					ten_don_vi: "BTL Bảo vệ Lăng Bác",
					culy: "750 m"
				},
				{
					ma_don_vi: "LU-144",
					ten_don_vi: "Lữ đoàn 144 BTTM",
					culy: "1.240 m"
				},
				{
					ma_don_vi: "BV-354",
					ten_don_vi: "Bệnh viện Quân y 354",
					culy: "3.120 m"
				},
				{
					ma_don_vi: "HV-KTQS",
					ten_don_vi: "Học viện Kỹ thuật Quân sự",
					culy: "6.050 m"
				}
			]
		},
		{
			id: 2,
			title: "ST_Buffer: Tạo vùng đệm phòng thủ 5.000m quanh Học viện KTQS",
			sql: `SELECT ma_don_vi, ten_don_vi,
       ST_AsGeoJSON(ST_Transform(ST_Buffer(geom_32648, 5000), 4326)) AS buffer_geojson_wgs84
FROM don_vi_quan_doi
WHERE ma_don_vi = 'HV-KTQS';`,
			resultSample: [{
				ma_don_vi: "HV-KTQS",
				ten_don_vi: "Học viện Kỹ thuật Quân sự",
				culy: "Buffer Polygon: 64 vertices, R=5.000m (Diện tích: 78.54 km²)"
			}]
		},
		{
			id: 3,
			title: "Thống kê mật độ đơn vị quân sự theo từng Quận/Huyện Hà Nội",
			sql: `SELECT quan_huyen, COUNT(*) AS so_luong_don_vi,
       ARRAY_AGG(ten_viet_tat) AS cac_don_vi
FROM don_vi_quan_doi
GROUP BY quan_huyen
ORDER BY so_luong_don_vi DESC;`,
			resultSample: [
				{
					ma_don_vi: "Q. Ba Đình",
					ten_don_vi: "5 đơn vị (BQP, BTL Lăng, Lữ 144, BV 354, Ban CHQS)",
					culy: "Khu vực lõi"
				},
				{
					ma_don_vi: "Q. Cầu Giấy",
					ten_don_vi: "4 đơn vị (HV Quốc phòng, BTL 86, Sư đoàn 361, Ban CHQS)",
					culy: "Phía Tây"
				},
				{
					ma_don_vi: "TX. Sơn Tây",
					ten_don_vi: "3 đơn vị (Trường SQ Lục quân 1, BTL Pháo binh, Ban CHQS)",
					culy: "Căn cứ phía Tây"
				}
			]
		}
	];
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "fixed inset-0 z-[2000] flex items-center justify-center bg-black/80 backdrop-blur-sm p-4 overflow-y-auto",
		children: /* @__PURE__ */ _jsxDEV("div", {
			className: "relative w-full max-w-4xl bg-[#09101f] border border-slate-700 rounded-xl shadow-2xl text-white overflow-hidden my-6",
			children: [
				/* @__PURE__ */ _jsxDEV("div", {
					className: "p-4 bg-gradient-to-r from-slate-900 via-slate-800 to-slate-900 border-b border-slate-700 flex items-center justify-between",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center gap-2.5",
						children: [/* @__PURE__ */ _jsxDEV(Database, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 104,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", { children: /* @__PURE__ */ _jsxDEV("h3", {
							className: "text-base font-bold text-white flex items-center gap-2",
							children: [/* @__PURE__ */ _jsxDEV("span", { children: "Cơ sở Dữ liệu Không gian PostGIS (Spatial Database Engine)" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 107,
								columnNumber: 17
							}, this), /* @__PURE__ */ _jsxDEV("span", {
								className: "text-xs font-mono px-2 py-0.5 rounded bg-emerald-950 text-emerald-300 border border-emerald-700",
								children: "PostGIS 3.3+ / PostgreSQL 15+"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 108,
								columnNumber: 17
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 106,
							columnNumber: 15
						}, this) }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 105,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 103,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("button", {
						onClick: onClose,
						className: "text-slate-400 hover:text-white p-1",
						children: /* @__PURE__ */ _jsxDEV(X, { className: "w-5 h-5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 115,
							columnNumber: 13
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 114,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 102,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "flex border-b border-slate-800 bg-[#060b14] px-4 gap-2 text-xs font-semibold",
					children: [
						/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setActiveTab("info"),
							className: `py-3 px-3 border-b-2 flex items-center gap-1.5 transition ${activeTab === "info" ? "border-emerald-500 text-emerald-300 font-bold" : "border-transparent text-slate-400 hover:text-white"}`,
							children: [/* @__PURE__ */ _jsxDEV(Server, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 129,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Thông Số Kết Nối & Kiến Trúc CSDL" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 130,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 121,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setActiveTab("query"),
							className: `py-3 px-3 border-b-2 flex items-center gap-1.5 transition ${activeTab === "query" ? "border-emerald-500 text-emerald-300 font-bold" : "border-transparent text-slate-400 hover:text-white"}`,
							children: [/* @__PURE__ */ _jsxDEV(Terminal, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 140,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Mẫu Truy Vấn Không Gian (Spatial Queries)" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 141,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 132,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setActiveTab("schema"),
							className: `py-3 px-3 border-b-2 flex items-center gap-1.5 transition ${activeTab === "schema" ? "border-emerald-500 text-emerald-300 font-bold" : "border-transparent text-slate-400 hover:text-white"}`,
							children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 151,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: [
								"Mã DDL PostGIS SQL Toàn Bộ (",
								units.length,
								" bản ghi)"
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 152,
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
					lineNumber: 120,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "p-6 space-y-4 max-h-[75vh] overflow-y-auto text-xs",
					children: [
						activeTab === "info" && /* @__PURE__ */ _jsxDEV("div", {
							className: "space-y-4",
							children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "grid grid-cols-1 md:grid-cols-2 gap-4",
								children: [/* @__PURE__ */ _jsxDEV("div", {
									className: "bg-[#0e1628] border border-slate-800 rounded-lg p-4 space-y-2.5",
									children: [/* @__PURE__ */ _jsxDEV("h4", {
										className: "text-xs font-bold uppercase text-emerald-400 flex items-center gap-2",
										children: [/* @__PURE__ */ _jsxDEV(Server, { className: "w-4 h-4" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 165,
											columnNumber: 21
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Thông số cấu hình PostGIS" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 166,
											columnNumber: 21
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 164,
										columnNumber: 19
									}, this), /* @__PURE__ */ _jsxDEV("div", {
										className: "font-mono text-slate-300 space-y-1.5 text-[11px]",
										children: [
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between border-b border-slate-800 pb-1",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "Host:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 170,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-white",
													children: "localhost / 127.0.0.1"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 171,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 169,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between border-b border-slate-800 pb-1",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "Port:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 174,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-emerald-400",
													children: "5432"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 175,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 173,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between border-b border-slate-800 pb-1",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "Database Name:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 178,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-white",
													children: "hanoi_military_gis"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 179,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 177,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between border-b border-slate-800 pb-1",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "Spatial Extension:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 182,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-emerald-400",
													children: "postgis, postgis_topology"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 183,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 181,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between border-b border-slate-800 pb-1",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "Geometry Table:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 186,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-sky-300",
													children: "don_vi_quan_doi"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 187,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 185,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between border-b border-slate-800 pb-1",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "SRID Chuẩn:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 190,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-amber-400",
													children: "EPSG:32648 (VN-2000 UTM 48N)"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 191,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 189,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("div", {
												className: "flex justify-between",
												children: [/* @__PURE__ */ _jsxDEV("span", {
													className: "text-slate-400 font-sans",
													children: "Trạng thái đồng bộ:"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 194,
													columnNumber: 23
												}, this), /* @__PURE__ */ _jsxDEV("span", {
													className: "text-emerald-400 font-bold flex items-center gap-1",
													children: [/* @__PURE__ */ _jsxDEV("span", { className: "w-2 h-2 rounded-full bg-emerald-400 animate-pulse" }, void 0, false, {
														fileName: _jsxFileName,
														lineNumber: 196,
														columnNumber: 25
													}, this), "Đã sẵn sàng kết nối & truy vấn"]
												}, void 0, true, {
													fileName: _jsxFileName,
													lineNumber: 195,
													columnNumber: 23
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 193,
												columnNumber: 21
											}, this)
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 168,
										columnNumber: 19
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 163,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "bg-[#0e1628] border border-slate-800 rounded-lg p-4 space-y-2.5",
									children: [/* @__PURE__ */ _jsxDEV("h4", {
										className: "text-xs font-bold uppercase text-sky-400 flex items-center gap-2",
										children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-4 h-4" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 206,
											columnNumber: 21
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Cấu trúc thực thể không gian" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 207,
											columnNumber: 21
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 205,
										columnNumber: 19
									}, this), /* @__PURE__ */ _jsxDEV("div", {
										className: "text-[11px] text-slate-300 space-y-2",
										children: [/* @__PURE__ */ _jsxDEV("p", { children: [
											"Bảng ",
											/* @__PURE__ */ _jsxDEV("code", {
												className: "text-emerald-400 font-mono",
												children: "don_vi_quan_doi"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 211,
												columnNumber: 28
											}, this),
											" được thiết kế theo mô hình lai (Hybrid Spatial Model):"
										] }, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 210,
											columnNumber: 21
										}, this), /* @__PURE__ */ _jsxDEV("ul", {
											className: "list-disc pl-4 space-y-1 text-slate-300",
											children: [
												/* @__PURE__ */ _jsxDEV("li", { children: [/* @__PURE__ */ _jsxDEV("strong", {
													className: "text-white font-mono",
													children: "geom_32648 (Point, 32648)"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 216,
													columnNumber: 25
												}, this), ": Hệ tọa độ phẳng UTM 48N phục vụ tính khoảng cách cơ động đường bộ (m) và vẽ vùng đệm buffer (m) không bị biến dạng."] }, void 0, true, {
													fileName: _jsxFileName,
													lineNumber: 215,
													columnNumber: 23
												}, this),
												/* @__PURE__ */ _jsxDEV("li", { children: [/* @__PURE__ */ _jsxDEV("strong", {
													className: "text-white font-mono",
													children: "geom_4326 (Point, 4326)"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 221,
													columnNumber: 25
												}, this), ": Hệ tọa độ trắc địa WGS-84 phục vụ trực tiếp cho Web GIS, bản đồ vệ tinh và GPS."] }, void 0, true, {
													fileName: _jsxFileName,
													lineNumber: 220,
													columnNumber: 23
												}, this),
												/* @__PURE__ */ _jsxDEV("li", { children: [/* @__PURE__ */ _jsxDEV("strong", {
													className: "text-white font-mono",
													children: "Chỉ mục GIST (Spatial Index)"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 226,
													columnNumber: 25
												}, this), ": Tăng tốc độ truy vấn không gian lên gấp 50-100 lần."] }, void 0, true, {
													fileName: _jsxFileName,
													lineNumber: 225,
													columnNumber: 23
												}, this)
											]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 214,
											columnNumber: 21
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 209,
										columnNumber: 19
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 204,
									columnNumber: 17
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 161,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("div", {
								className: "flex flex-wrap items-center justify-between gap-3 p-4 bg-slate-900 border border-slate-800 rounded-lg",
								children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h5", {
									className: "font-bold text-white text-xs",
									children: "Tải về toàn bộ kịch bản PostGIS SQL"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 237,
									columnNumber: 19
								}, this), /* @__PURE__ */ _jsxDEV("p", {
									className: "text-[11px] text-slate-400 mt-0.5",
									children: "Chạy trực tiếp trên pgAdmin 4, DBeaver hoặc dòng lệnh psql"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 238,
									columnNumber: 19
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 236,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "flex gap-2",
									children: [/* @__PURE__ */ _jsxDEV("button", {
										onClick: handleCopy,
										className: "px-3 py-1.5 rounded bg-slate-800 hover:bg-slate-700 text-slate-300 text-xs font-semibold flex items-center gap-1.5",
										children: [copied ? /* @__PURE__ */ _jsxDEV(Check, { className: "w-3.5 h-3.5 text-emerald-400" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 247,
											columnNumber: 31
										}, this) : /* @__PURE__ */ _jsxDEV(Copy, { className: "w-3.5 h-3.5" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 247,
											columnNumber: 84
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: copied ? "Đã sao chép!" : "Sao chép SQL" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 248,
											columnNumber: 21
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 243,
										columnNumber: 19
									}, this), /* @__PURE__ */ _jsxDEV("button", {
										onClick: handleDownload,
										className: "px-3.5 py-1.5 rounded bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold flex items-center gap-1.5 shadow",
										children: [/* @__PURE__ */ _jsxDEV(Download, { className: "w-3.5 h-3.5" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 254,
											columnNumber: 21
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tải file .sql" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 255,
											columnNumber: 21
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 250,
										columnNumber: 19
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 242,
									columnNumber: 17
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 235,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 160,
							columnNumber: 13
						}, this),
						activeTab === "query" && /* @__PURE__ */ _jsxDEV("div", {
							className: "space-y-4",
							children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "flex gap-2",
								children: demoQueries.map((q) => /* @__PURE__ */ _jsxDEV("button", {
									onClick: () => setSelectedDemoQuery(q.id),
									className: `flex-1 p-2 rounded-lg text-left border transition ${selectedDemoQuery === q.id ? "bg-emerald-950/60 border-emerald-500 text-emerald-200 font-bold" : "bg-slate-900 border-slate-800 text-slate-400 hover:bg-slate-800"}`,
									children: [/* @__PURE__ */ _jsxDEV("div", {
										className: "text-[10px] uppercase font-mono",
										children: ["Truy vấn #", q.id]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 276,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("div", {
										className: "text-xs truncate text-white",
										children: q.title
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 277,
										columnNumber: 21
									}, this)]
								}, q.id, true, {
									fileName: _jsxFileName,
									lineNumber: 267,
									columnNumber: 19
								}, this))
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 265,
								columnNumber: 15
							}, this), (() => {
								const cur = demoQueries.find((q) => q.id === selectedDemoQuery);
								return /* @__PURE__ */ _jsxDEV("div", {
									className: "space-y-3",
									children: [/* @__PURE__ */ _jsxDEV("div", {
										className: "bg-[#050912] border border-slate-800 rounded-lg p-3",
										children: [/* @__PURE__ */ _jsxDEV("div", {
											className: "flex justify-between items-center mb-1.5 text-slate-400 text-[10px] font-mono",
											children: [/* @__PURE__ */ _jsxDEV("span", { children: "CÂU LỆNH POSTGIS SQL:" }, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 289,
												columnNumber: 25
											}, this), /* @__PURE__ */ _jsxDEV("button", {
												onClick: () => {
													navigator.clipboard.writeText(cur.sql);
													alert("Đã copy câu truy vấn SQL!");
												},
												className: "hover:text-emerald-400 flex items-center gap-1",
												children: [/* @__PURE__ */ _jsxDEV(Copy, { className: "w-3 h-3" }, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 297,
													columnNumber: 27
												}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Sao chép" }, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 298,
													columnNumber: 27
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 290,
												columnNumber: 25
											}, this)]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 288,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("pre", {
											className: "font-mono text-emerald-400 text-xs overflow-x-auto whitespace-pre-wrap leading-relaxed",
											children: cur.sql
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 301,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 287,
										columnNumber: 21
									}, this), /* @__PURE__ */ _jsxDEV("div", {
										className: "bg-[#0e1628] border border-slate-800 rounded-lg p-3 space-y-2",
										children: [/* @__PURE__ */ _jsxDEV("div", {
											className: "text-[11px] font-bold uppercase text-slate-300",
											children: "Kết quả mẫu trả về từ cơ sở dữ liệu:"
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 308,
											columnNumber: 23
										}, this), /* @__PURE__ */ _jsxDEV("div", {
											className: "overflow-x-auto",
											children: /* @__PURE__ */ _jsxDEV("table", {
												className: "w-full text-left font-mono text-xs text-slate-300",
												children: [/* @__PURE__ */ _jsxDEV("thead", {
													className: "bg-[#090f1d] text-slate-400 text-[10px] border-b border-slate-800",
													children: /* @__PURE__ */ _jsxDEV("tr", { children: [
														/* @__PURE__ */ _jsxDEV("th", {
															className: "p-2",
															children: "Mã / Tiêu chí"
														}, void 0, false, {
															fileName: _jsxFileName,
															lineNumber: 315,
															columnNumber: 31
														}, this),
														/* @__PURE__ */ _jsxDEV("th", {
															className: "p-2",
															children: "Tên đơn vị / Chi tiết"
														}, void 0, false, {
															fileName: _jsxFileName,
															lineNumber: 316,
															columnNumber: 31
														}, this),
														/* @__PURE__ */ _jsxDEV("th", {
															className: "p-2",
															children: "Khoảng cách / Ghi chú"
														}, void 0, false, {
															fileName: _jsxFileName,
															lineNumber: 317,
															columnNumber: 31
														}, this)
													] }, void 0, true, {
														fileName: _jsxFileName,
														lineNumber: 314,
														columnNumber: 29
													}, this)
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 313,
													columnNumber: 27
												}, this), /* @__PURE__ */ _jsxDEV("tbody", {
													className: "divide-y divide-slate-800",
													children: cur.resultSample.map((row, idx) => /* @__PURE__ */ _jsxDEV("tr", {
														className: "hover:bg-slate-800/40",
														children: [
															/* @__PURE__ */ _jsxDEV("td", {
																className: "p-2 font-bold text-sky-400",
																children: row.ma_don_vi
															}, void 0, false, {
																fileName: _jsxFileName,
																lineNumber: 323,
																columnNumber: 33
															}, this),
															/* @__PURE__ */ _jsxDEV("td", {
																className: "p-2 text-white",
																children: row.ten_don_vi
															}, void 0, false, {
																fileName: _jsxFileName,
																lineNumber: 324,
																columnNumber: 33
															}, this),
															/* @__PURE__ */ _jsxDEV("td", {
																className: "p-2 text-emerald-300",
																children: row.culy
															}, void 0, false, {
																fileName: _jsxFileName,
																lineNumber: 325,
																columnNumber: 33
															}, this)
														]
													}, idx, true, {
														fileName: _jsxFileName,
														lineNumber: 322,
														columnNumber: 31
													}, this))
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 320,
													columnNumber: 27
												}, this)]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 312,
												columnNumber: 25
											}, this)
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 311,
											columnNumber: 23
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 307,
										columnNumber: 21
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 286,
									columnNumber: 19
								}, this);
							})()]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 264,
							columnNumber: 13
						}, this),
						activeTab === "schema" && /* @__PURE__ */ _jsxDEV("div", {
							className: "space-y-3",
							children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "flex justify-between items-center",
								children: [/* @__PURE__ */ _jsxDEV("span", {
									className: "text-xs text-slate-400",
									children: [
										"Script bao gồm khởi tạo bảng, thiết lập Spatial GIST Indexes và INSERT ",
										units.length,
										" bản ghi"
									]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 342,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("button", {
									onClick: handleCopy,
									className: "px-3 py-1 rounded bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs flex items-center gap-1",
									children: [/* @__PURE__ */ _jsxDEV(Copy, { className: "w-3.5 h-3.5" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 349,
										columnNumber: 19
									}, this), /* @__PURE__ */ _jsxDEV("span", { children: copied ? "Đã sao chép" : "Sao chép toàn bộ" }, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 350,
										columnNumber: 19
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 345,
									columnNumber: 17
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 341,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("pre", {
								className: "p-4 bg-[#050912] border border-slate-800 rounded-lg font-mono text-emerald-400 text-[11px] max-h-96 overflow-y-auto whitespace-pre-wrap leading-relaxed",
								children: fullSQL
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 353,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 340,
							columnNumber: 13
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 157,
					columnNumber: 9
				}, this),
				/* @__PURE__ */ _jsxDEV("div", {
					className: "p-4 bg-slate-900 border-t border-slate-800 flex justify-end",
					children: /* @__PURE__ */ _jsxDEV("button", {
						onClick: onClose,
						className: "px-4 py-2 rounded bg-slate-800 hover:bg-slate-700 text-white font-medium text-xs",
						children: "Đóng cửa sổ"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 362,
						columnNumber: 11
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 361,
					columnNumber: 9
				}, this)
			]
		}, void 0, true, {
			fileName: _jsxFileName,
			lineNumber: 100,
			columnNumber: 7
		}, this)
	}, void 0, false, {
		fileName: _jsxFileName,
		lineNumber: 99,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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Ozs7O0dBQ0Y7Ozs7OztDQUNGOzs7OztBQUVUIiwibmFtZXMiOltdLCJzb3VyY2VzIjpbIlBvc3RnaXNNb2RhbC50c3giXSwidmVyc2lvbiI6Mywic291cmNlc0NvbnRlbnQiOlsiaW1wb3J0IFJlYWN0LCB7IHVzZVN0YXRlIH0gZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0IH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IGdlbmVyYXRlUG9zdGdpc1NRTCB9IGZyb20gJy4uL3V0aWxzL3Bvc3RnaXNFeHBvcnQnO1xuaW1wb3J0IHtcbiAgRGF0YWJhc2UsXG4gIFgsXG4gIENvcHksXG4gIENoZWNrLFxuICBEb3dubG9hZCxcbiAgVGVybWluYWwsXG4gIFNlcnZlcixcbiAgUGxheSxcbiAgTGF5ZXJzLFxufSBmcm9tICdsdWNpZGUtcmVhY3QnO1xuXG5pbnRlcmZhY2UgUG9zdGdpc01vZGFsUHJvcHMge1xuICBpc09wZW46IGJvb2xlYW47XG4gIG9uQ2xvc2U6ICgpID0+IHZvaWQ7XG4gIHVuaXRzOiBNaWxpdGFyeVVuaXRbXTtcbn1cblxuZXhwb3J0IGNvbnN0IFBvc3RnaXNNb2RhbDogUmVhY3QuRkM8UG9zdGdpc01vZGFsUHJvcHM+ID0gKHsgaXNPcGVuLCBvbkNsb3NlLCB1bml0cyB9KSA9PiB7XG4gIGlmICghaXNPcGVuKSByZXR1cm4gbnVsbDtcblxuICBjb25zdCBbY29waWVkLCBzZXRDb3BpZWRdID0gdXNlU3RhdGUoZmFsc2UpO1xuICBjb25zdCBbYWN0aXZlVGFiLCBzZXRBY3RpdmVUYWJdID0gdXNlU3RhdGU8J2luZm8nIHwgJ3NjaGVtYScgfCAncXVlcnknPignaW5mbycpO1xuICBjb25zdCBbc2VsZWN0ZWREZW1vUXVlcnksIHNldFNlbGVjdGVkRGVtb1F1ZXJ5XSA9IHVzZVN0YXRlPG51bWJlcj4oMSk7XG5cbiAgY29uc3QgZnVsbFNRTCA9IGdlbmVyYXRlUG9zdGdpc1NRTCh1bml0cyk7XG5cbiAgY29uc3QgaGFuZGxlQ29weSA9ICgpID0+IHtcbiAgICBuYXZpZ2F0b3IuY2xpcGJvYXJkLndyaXRlVGV4dChmdWxsU1FMKTtcbiAgICBzZXRDb3BpZWQodHJ1ZSk7XG4gICAgc2V0VGltZW91dCgoKSA9PiBzZXRDb3BpZWQoZmFsc2UpLCAyMDAwKTtcbiAgfTtcblxuICBjb25zdCBoYW5kbGVEb3dubG9hZCA9ICgpID0+IHtcbiAgICBjb25zdCBibG9iID0gbmV3IEJsb2IoW2Z1bGxTUUxdLCB7IHR5cGU6ICd0ZXh0L3NxbCcgfSk7XG4gICAgY29uc3QgdXJsID0gVVJMLmNyZWF0ZU9iamVjdFVSTChibG9iKTtcbiAgICBjb25zdCBhID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnYScpO1xuICAgIGEuaHJlZiA9IHVybDtcbiAgICBhLmRvd25sb2FkID0gJ2hhbm9pX21pbGl0YXJ5X2dpc19wb3N0Z2lzLnNxbCc7XG4gICAgZG9jdW1lbnQuYm9keS5hcHBlbmRDaGlsZChhKTtcbiAgICBhLmNsaWNrKCk7XG4gICAgZG9jdW1lbnQuYm9keS5yZW1vdmVDaGlsZChhKTtcbiAgICBVUkwucmV2b2tlT2JqZWN0VVJMKHVybCk7XG4gIH07XG5cbiAgLy8gRGVtbyBxdWVyaWVzXG4gIGNvbnN0IGRlbW9RdWVyaWVzID0gW1xuICAgIHtcbiAgICAgIGlkOiAxLFxuICAgICAgdGl0bGU6ICdTVF9EV2l0aGluOiBUw6xtIMSRxqFuIHbhu4sgdHJvbmcgYsOhbiBrw61uaCAxMCBrbSBxdWFuaCBT4bufIENo4buJIGh1eSBCUVAnLFxuICAgICAgc3FsOiBgU0VMRUNUIG1hX2Rvbl92aSwgdGVuX2Rvbl92aSwgcXVhbl9iaW5oX2NodW5nLFxuICAgICAgIFJPVU5EKFNUX0Rpc3RhbmNlKGdlb21fMzI2NDgsIChTRUxFQ1QgZ2VvbV8zMjY0OCBGUk9NIGRvbl92aV9xdWFuX2RvaSBXSEVSRSBtYV9kb25fdmkgPSAnQlFQLUhRJykpOjpudW1lcmljLCAyKSBBUyBraG9hbmdfY2FjaF9tXG5GUk9NIGRvbl92aV9xdWFuX2RvaVxuV0hFUkUgU1RfRFdpdGhpbihnZW9tXzMyNjQ4LCAoU0VMRUNUIGdlb21fMzI2NDggRlJPTSBkb25fdmlfcXVhbl9kb2kgV0hFUkUgbWFfZG9uX3ZpID0gJ0JRUC1IUScpLCAxMDAwMClcbk9SREVSIEJZIGtob2FuZ19jYWNoX20gQVNDO2AsXG4gICAgICByZXN1bHRTYW1wbGU6IFtcbiAgICAgICAgeyBtYV9kb25fdmk6ICdCUVAtSFEnLCB0ZW5fZG9uX3ZpOiAnU+G7nyBDaOG7iSBodXkgQuG7mSBRdeG7kWMgcGjDsm5nJywgY3VseTogJzAgbScgfSxcbiAgICAgICAgeyBtYV9kb25fdmk6ICdCVEwtTEFORycsIHRlbl9kb25fdmk6ICdCVEwgQuG6o28gduG7hyBMxINuZyBCw6FjJywgY3VseTogJzc1MCBtJyB9LFxuICAgICAgICB7IG1hX2Rvbl92aTogJ0xVLTE0NCcsIHRlbl9kb25fdmk6ICdM4buvIMSRb8OgbiAxNDQgQlRUTScsIGN1bHk6ICcxLjI0MCBtJyB9LFxuICAgICAgICB7IG1hX2Rvbl92aTogJ0JWLTM1NCcsIHRlbl9kb25fdmk6ICdC4buHbmggdmnhu4duIFF1w6JuIHkgMzU0JywgY3VseTogJzMuMTIwIG0nIH0sXG4gICAgICAgIHsgbWFfZG9uX3ZpOiAnSFYtS1RRUycsIHRlbl9kb25fdmk6ICdI4buNYyB2aeG7h24gS+G7uSB0aHXhuq10IFF1w6JuIHPhu7EnLCBjdWx5OiAnNi4wNTAgbScgfSxcbiAgICAgIF0sXG4gICAgfSxcbiAgICB7XG4gICAgICBpZDogMixcbiAgICAgIHRpdGxlOiAnU1RfQnVmZmVyOiBU4bqhbyB2w7luZyDEkeG7h20gcGjDsm5nIHRo4bunIDUuMDAwbSBxdWFuaCBI4buNYyB2aeG7h24gS1RRUycsXG4gICAgICBzcWw6IGBTRUxFQ1QgbWFfZG9uX3ZpLCB0ZW5fZG9uX3ZpLFxuICAgICAgIFNUX0FzR2VvSlNPTihTVF9UcmFuc2Zvcm0oU1RfQnVmZmVyKGdlb21fMzI2NDgsIDUwMDApLCA0MzI2KSkgQVMgYnVmZmVyX2dlb2pzb25fd2dzODRcbkZST00gZG9uX3ZpX3F1YW5fZG9pXG5XSEVSRSBtYV9kb25fdmkgPSAnSFYtS1RRUyc7YCxcbiAgICAgIHJlc3VsdFNhbXBsZTogW1xuICAgICAgICB7XG4gICAgICAgICAgbWFfZG9uX3ZpOiAnSFYtS1RRUycsXG4gICAgICAgICAgdGVuX2Rvbl92aTogJ0jhu41jIHZp4buHbiBL4bu5IHRodeG6rXQgUXXDom4gc+G7sScsXG4gICAgICAgICAgY3VseTogJ0J1ZmZlciBQb2x5Z29uOiA2NCB2ZXJ0aWNlcywgUj01LjAwMG0gKERp4buHbiB0w61jaDogNzguNTQga23CsiknLFxuICAgICAgICB9LFxuICAgICAgXSxcbiAgICB9LFxuICAgIHtcbiAgICAgIGlkOiAzLFxuICAgICAgdGl0bGU6ICdUaOG7kW5nIGvDqiBt4bqtdCDEkeG7mSDEkcahbiB24buLIHF1w6JuIHPhu7EgdGhlbyB04burbmcgUXXhuq1uL0h1eeG7h24gSMOgIE7hu5lpJyxcbiAgICAgIHNxbDogYFNFTEVDVCBxdWFuX2h1eWVuLCBDT1VOVCgqKSBBUyBzb19sdW9uZ19kb25fdmksXG4gICAgICAgQVJSQVlfQUdHKHRlbl92aWV0X3RhdCkgQVMgY2FjX2Rvbl92aVxuRlJPTSBkb25fdmlfcXVhbl9kb2lcbkdST1VQIEJZIHF1YW5faHV5ZW5cbk9SREVSIEJZIHNvX2x1b25nX2Rvbl92aSBERVNDO2AsXG4gICAgICByZXN1bHRTYW1wbGU6IFtcbiAgICAgICAgeyBtYV9kb25fdmk6ICdRLiBCYSDEkMOsbmgnLCB0ZW5fZG9uX3ZpOiAnNSDEkcahbiB24buLIChCUVAsIEJUTCBMxINuZywgTOG7ryAxNDQsIEJWIDM1NCwgQmFuIENIUVMpJywgY3VseTogJ0todSB24buxYyBsw7VpJyB9LFxuICAgICAgICB7IG1hX2Rvbl92aTogJ1EuIEPhuqd1IEdp4bqleScsIHRlbl9kb25fdmk6ICc0IMSRxqFuIHbhu4sgKEhWIFF14buRYyBwaMOybmcsIEJUTCA4NiwgU8awIMSRb8OgbiAzNjEsIEJhbiBDSFFTKScsIGN1bHk6ICdQaMOtYSBUw6J5JyB9LFxuICAgICAgICB7IG1hX2Rvbl92aTogJ1RYLiBTxqFuIFTDonknLCB0ZW5fZG9uX3ZpOiAnMyDEkcahbiB24buLIChUcsaw4budbmcgU1EgTOG7pWMgcXXDom4gMSwgQlRMIFBow6FvIGJpbmgsIEJhbiBDSFFTKScsIGN1bHk6ICdDxINuIGPhu6kgcGjDrWEgVMOieScgfSxcbiAgICAgIF0sXG4gICAgfSxcbiAgXTtcblxuICByZXR1cm4gKFxuICAgIDxkaXYgY2xhc3NOYW1lPVwiZml4ZWQgaW5zZXQtMCB6LVsyMDAwXSBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciBiZy1ibGFjay84MCBiYWNrZHJvcC1ibHVyLXNtIHAtNCBvdmVyZmxvdy15LWF1dG9cIj5cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwicmVsYXRpdmUgdy1mdWxsIG1heC13LTR4bCBiZy1bIzA5MTAxZl0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC14bCBzaGFkb3ctMnhsIHRleHQtd2hpdGUgb3ZlcmZsb3ctaGlkZGVuIG15LTZcIj5cbiAgICAgICAgey8qIEhlYWRlciAqL31cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJwLTQgYmctZ3JhZGllbnQtdG8tciBmcm9tLXNsYXRlLTkwMCB2aWEtc2xhdGUtODAwIHRvLXNsYXRlLTkwMCBib3JkZXItYiBib3JkZXItc2xhdGUtNzAwIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlblwiPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTIuNVwiPlxuICAgICAgICAgICAgPERhdGFiYXNlIGNsYXNzTmFtZT1cInctNSBoLTUgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8aDMgY2xhc3NOYW1lPVwidGV4dC1iYXNlIGZvbnQtYm9sZCB0ZXh0LXdoaXRlIGZsZXggaXRlbXMtY2VudGVyIGdhcC0yXCI+XG4gICAgICAgICAgICAgICAgPHNwYW4+Q8ahIHPhu58gROG7ryBsaeG7h3UgS2jDtG5nIGdpYW4gUG9zdEdJUyAoU3BhdGlhbCBEYXRhYmFzZSBFbmdpbmUpPC9zcGFuPlxuICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1tb25vIHB4LTIgcHktMC41IHJvdW5kZWQgYmctZW1lcmFsZC05NTAgdGV4dC1lbWVyYWxkLTMwMCBib3JkZXIgYm9yZGVyLWVtZXJhbGQtNzAwXCI+XG4gICAgICAgICAgICAgICAgICBQb3N0R0lTIDMuMysgLyBQb3N0Z3JlU1FMIDE1K1xuICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgPC9oMz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxidXR0b24gb25DbGljaz17b25DbG9zZX0gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC13aGl0ZSBwLTFcIj5cbiAgICAgICAgICAgIDxYIGNsYXNzTmFtZT1cInctNSBoLTVcIiAvPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogVGFiIENvbnRyb2xzICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBiZy1bIzA2MGIxNF0gcHgtNCBnYXAtMiB0ZXh0LXhzIGZvbnQtc2VtaWJvbGRcIj5cbiAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRBY3RpdmVUYWIoJ2luZm8nKX1cbiAgICAgICAgICAgIGNsYXNzTmFtZT17YHB5LTMgcHgtMyBib3JkZXItYi0yIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xLjUgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgICBhY3RpdmVUYWIgPT09ICdpbmZvJ1xuICAgICAgICAgICAgICAgID8gJ2JvcmRlci1lbWVyYWxkLTUwMCB0ZXh0LWVtZXJhbGQtMzAwIGZvbnQtYm9sZCdcbiAgICAgICAgICAgICAgICA6ICdib3JkZXItdHJhbnNwYXJlbnQgdGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC13aGl0ZSdcbiAgICAgICAgICAgIH1gfVxuICAgICAgICAgID5cbiAgICAgICAgICAgIDxTZXJ2ZXIgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5UaMO0bmcgU+G7kSBL4bq/dCBO4buRaSAmIEtp4bq/biBUcsO6YyBDU0RMPC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldEFjdGl2ZVRhYigncXVlcnknKX1cbiAgICAgICAgICAgIGNsYXNzTmFtZT17YHB5LTMgcHgtMyBib3JkZXItYi0yIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xLjUgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgICBhY3RpdmVUYWIgPT09ICdxdWVyeSdcbiAgICAgICAgICAgICAgICA/ICdib3JkZXItZW1lcmFsZC01MDAgdGV4dC1lbWVyYWxkLTMwMCBmb250LWJvbGQnXG4gICAgICAgICAgICAgICAgOiAnYm9yZGVyLXRyYW5zcGFyZW50IHRleHQtc2xhdGUtNDAwIGhvdmVyOnRleHQtd2hpdGUnXG4gICAgICAgICAgICB9YH1cbiAgICAgICAgICA+XG4gICAgICAgICAgICA8VGVybWluYWwgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5N4bqrdSBUcnV5IFbhuqVuIEtow7RuZyBHaWFuIChTcGF0aWFsIFF1ZXJpZXMpPC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldEFjdGl2ZVRhYignc2NoZW1hJyl9XG4gICAgICAgICAgICBjbGFzc05hbWU9e2BweS0zIHB4LTMgYm9yZGVyLWItMiBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41IHRyYW5zaXRpb24gJHtcbiAgICAgICAgICAgICAgYWN0aXZlVGFiID09PSAnc2NoZW1hJ1xuICAgICAgICAgICAgICAgID8gJ2JvcmRlci1lbWVyYWxkLTUwMCB0ZXh0LWVtZXJhbGQtMzAwIGZvbnQtYm9sZCdcbiAgICAgICAgICAgICAgICA6ICdib3JkZXItdHJhbnNwYXJlbnQgdGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC13aGl0ZSdcbiAgICAgICAgICAgIH1gfVxuICAgICAgICAgID5cbiAgICAgICAgICAgIDxMYXllcnMgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5Nw6MgRERMIFBvc3RHSVMgU1FMIFRvw6BuIELhu5kgKHt1bml0cy5sZW5ndGh9IGLhuqNuIGdoaSk8L3NwYW4+XG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIHsvKiBDb250ZW50IEFyZWEgKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC02IHNwYWNlLXktNCBtYXgtaC1bNzV2aF0gb3ZlcmZsb3cteS1hdXRvIHRleHQteHNcIj5cbiAgICAgICAgICB7LyogVEFCIDE6IElORk8gJiBBUkNISVRFQ1RVUkUgKi99XG4gICAgICAgICAge2FjdGl2ZVRhYiA9PT0gJ2luZm8nICYmIChcbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS00XCI+XG4gICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMSBtZDpncmlkLWNvbHMtMiBnYXAtNFwiPlxuICAgICAgICAgICAgICAgIHsvKiBDb25uZWN0aW9uIHBhcmFtcyAqL31cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLWxnIHAtNCBzcGFjZS15LTIuNVwiPlxuICAgICAgICAgICAgICAgICAgPGg0IGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1ib2xkIHVwcGVyY2FzZSB0ZXh0LWVtZXJhbGQtNDAwIGZsZXggaXRlbXMtY2VudGVyIGdhcC0yXCI+XG4gICAgICAgICAgICAgICAgICAgIDxTZXJ2ZXIgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICAgICAgICAgIDxzcGFuPlRow7RuZyBz4buRIGPhuqV1IGjDrG5oIFBvc3RHSVM8L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8L2g0PlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmb250LW1vbm8gdGV4dC1zbGF0ZS0zMDAgc3BhY2UteS0xLjUgdGV4dC1bMTFweF1cIj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGp1c3RpZnktYmV0d2VlbiBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIHBiLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBmb250LXNhbnNcIj5Ib3N0Ojwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXdoaXRlXCI+bG9jYWxob3N0IC8gMTI3LjAuMC4xPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGp1c3RpZnktYmV0d2VlbiBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIHBiLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBmb250LXNhbnNcIj5Qb3J0Ojwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LWVtZXJhbGQtNDAwXCI+NTQzMjwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi0xXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDAgZm9udC1zYW5zXCI+RGF0YWJhc2UgTmFtZTo8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC13aGl0ZVwiPmhhbm9pX21pbGl0YXJ5X2dpczwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi0xXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDAgZm9udC1zYW5zXCI+U3BhdGlhbCBFeHRlbnNpb246PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtZW1lcmFsZC00MDBcIj5wb3N0Z2lzLCBwb3N0Z2lzX3RvcG9sb2d5PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGp1c3RpZnktYmV0d2VlbiBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIHBiLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBmb250LXNhbnNcIj5HZW9tZXRyeSBUYWJsZTo8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1za3ktMzAwXCI+ZG9uX3ZpX3F1YW5fZG9pPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGp1c3RpZnktYmV0d2VlbiBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIHBiLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBmb250LXNhbnNcIj5TUklEIENodeG6qW46PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtYW1iZXItNDAwXCI+RVBTRzozMjY0OCAoVk4tMjAwMCBVVE0gNDhOKTwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW5cIj5cbiAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMCBmb250LXNhbnNcIj5UcuG6oW5nIHRow6FpIMSR4buTbmcgYuG7mTo8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1lbWVyYWxkLTQwMCBmb250LWJvbGQgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInctMiBoLTIgcm91bmRlZC1mdWxsIGJnLWVtZXJhbGQtNDAwIGFuaW1hdGUtcHVsc2VcIiAvPlxuICAgICAgICAgICAgICAgICAgICAgICAgxJDDoyBz4bq1biBzw6BuZyBr4bq/dCBu4buRaSAmIHRydXkgduG6pW5cbiAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICB7LyogRGF0YWJhc2UgQXJjaGl0ZWN0dXJlIFNjaGVtYSBkaWFncmFtICovfVxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQtbGcgcC00IHNwYWNlLXktMi41XCI+XG4gICAgICAgICAgICAgICAgICA8aDQgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdXBwZXJjYXNlIHRleHQtc2t5LTQwMCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgICAgICAgICA8TGF5ZXJzIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICAgICAgICAgICAgICA8c3Bhbj5D4bqldSB0csO6YyB0aOG7sWMgdGjhu4Mga2jDtG5nIGdpYW48L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8L2g0PlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSB0ZXh0LXNsYXRlLTMwMCBzcGFjZS15LTJcIj5cbiAgICAgICAgICAgICAgICAgICAgPHA+XG4gICAgICAgICAgICAgICAgICAgICAgQuG6o25nIDxjb2RlIGNsYXNzTmFtZT1cInRleHQtZW1lcmFsZC00MDAgZm9udC1tb25vXCI+ZG9uX3ZpX3F1YW5fZG9pPC9jb2RlPiDEkcaw4bujY1xuICAgICAgICAgICAgICAgICAgICAgIHRoaeG6v3Qga+G6vyB0aGVvIG3DtCBow6xuaCBsYWkgKEh5YnJpZCBTcGF0aWFsIE1vZGVsKTpcbiAgICAgICAgICAgICAgICAgICAgPC9wPlxuICAgICAgICAgICAgICAgICAgICA8dWwgY2xhc3NOYW1lPVwibGlzdC1kaXNjIHBsLTQgc3BhY2UteS0xIHRleHQtc2xhdGUtMzAwXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPGxpPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHN0cm9uZyBjbGFzc05hbWU9XCJ0ZXh0LXdoaXRlIGZvbnQtbW9ub1wiPmdlb21fMzI2NDggKFBvaW50LCAzMjY0OCk8L3N0cm9uZz46XG4gICAgICAgICAgICAgICAgICAgICAgICBI4buHIHThu41hIMSR4buZIHBo4bqzbmcgVVRNIDQ4TiBwaOG7pWMgduG7pSB0w61uaCBraG/huqNuZyBjw6FjaCBjxqEgxJHhu5luZyDEkcaw4budbmcgYuG7mSAobSkgdsOgIHbhur1cbiAgICAgICAgICAgICAgICAgICAgICAgIHbDuW5nIMSR4buHbSBidWZmZXIgKG0pIGtow7RuZyBi4buLIGJp4bq/biBk4bqhbmcuXG4gICAgICAgICAgICAgICAgICAgICAgPC9saT5cbiAgICAgICAgICAgICAgICAgICAgICA8bGk+XG4gICAgICAgICAgICAgICAgICAgICAgICA8c3Ryb25nIGNsYXNzTmFtZT1cInRleHQtd2hpdGUgZm9udC1tb25vXCI+Z2VvbV80MzI2IChQb2ludCwgNDMyNik8L3N0cm9uZz46XG4gICAgICAgICAgICAgICAgICAgICAgICBI4buHIHThu41hIMSR4buZIHRy4bqvYyDEkeG7i2EgV0dTLTg0IHBo4bulYyB24bulIHRy4buxYyB0aeG6v3AgY2hvIFdlYiBHSVMsIGLhuqNuIMSR4buTIHbhu4cgdGluaCB2w6BcbiAgICAgICAgICAgICAgICAgICAgICAgIEdQUy5cbiAgICAgICAgICAgICAgICAgICAgICA8L2xpPlxuICAgICAgICAgICAgICAgICAgICAgIDxsaT5cbiAgICAgICAgICAgICAgICAgICAgICAgIDxzdHJvbmcgY2xhc3NOYW1lPVwidGV4dC13aGl0ZSBmb250LW1vbm9cIj5DaOG7iSBt4bulYyBHSVNUIChTcGF0aWFsIEluZGV4KTwvc3Ryb25nPjpcbiAgICAgICAgICAgICAgICAgICAgICAgIFTEg25nIHThu5FjIMSR4buZIHRydXkgduG6pW4ga2jDtG5nIGdpYW4gbMOqbiBn4bqlcCA1MC0xMDAgbOG6p24uXG4gICAgICAgICAgICAgICAgICAgICAgPC9saT5cbiAgICAgICAgICAgICAgICAgICAgPC91bD5cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICB7LyogUXVpY2sgQWN0aW9ucyAqL31cbiAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGZsZXgtd3JhcCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGdhcC0zIHAtNCBiZy1zbGF0ZS05MDAgYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC1sZ1wiPlxuICAgICAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgICAgICA8aDUgY2xhc3NOYW1lPVwiZm9udC1ib2xkIHRleHQtd2hpdGUgdGV4dC14c1wiPlThuqNpIHbhu4EgdG/DoG4gYuG7mSBr4buLY2ggYuG6o24gUG9zdEdJUyBTUUw8L2g1PlxuICAgICAgICAgICAgICAgICAgPHAgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMC41XCI+XG4gICAgICAgICAgICAgICAgICAgIENo4bqheSB0cuG7sWMgdGnhur9wIHRyw6puIHBnQWRtaW4gNCwgREJlYXZlciBob+G6t2MgZMOybmcgbOG7h25oIHBzcWxcbiAgICAgICAgICAgICAgICAgIDwvcD5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggZ2FwLTJcIj5cbiAgICAgICAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgICAgICAgb25DbGljaz17aGFuZGxlQ29weX1cbiAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwicHgtMyBweS0xLjUgcm91bmRlZCBiZy1zbGF0ZS04MDAgaG92ZXI6Ymctc2xhdGUtNzAwIHRleHQtc2xhdGUtMzAwIHRleHQteHMgZm9udC1zZW1pYm9sZCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41XCJcbiAgICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgICAge2NvcGllZCA/IDxDaGVjayBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LWVtZXJhbGQtNDAwXCIgLz4gOiA8Q29weSBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNVwiIC8+fVxuICAgICAgICAgICAgICAgICAgICA8c3Bhbj57Y29waWVkID8gJ8SQw6Mgc2FvIGNow6lwIScgOiAnU2FvIGNow6lwIFNRTCd9PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZURvd25sb2FkfVxuICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJweC0zLjUgcHktMS41IHJvdW5kZWQgYmctZW1lcmFsZC02MDAgaG92ZXI6YmctZW1lcmFsZC01MDAgdGV4dC13aGl0ZSB0ZXh0LXhzIGZvbnQtc2VtaWJvbGQgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEuNSBzaGFkb3dcIlxuICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICA8RG93bmxvYWQgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjVcIiAvPlxuICAgICAgICAgICAgICAgICAgICA8c3Bhbj5U4bqjaSBmaWxlIC5zcWw8L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICApfVxuXG4gICAgICAgICAgey8qIFRBQiAyOiBTUEFUSUFMIFFVRVJJRVMgREVNTyAqL31cbiAgICAgICAgICB7YWN0aXZlVGFiID09PSAncXVlcnknICYmIChcbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS00XCI+XG4gICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBnYXAtMlwiPlxuICAgICAgICAgICAgICAgIHtkZW1vUXVlcmllcy5tYXAoKHEpID0+IChcbiAgICAgICAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgICAgICAga2V5PXtxLmlkfVxuICAgICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRTZWxlY3RlZERlbW9RdWVyeShxLmlkKX1cbiAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPXtgZmxleC0xIHAtMiByb3VuZGVkLWxnIHRleHQtbGVmdCBib3JkZXIgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgICAgICAgICAgIHNlbGVjdGVkRGVtb1F1ZXJ5ID09PSBxLmlkXG4gICAgICAgICAgICAgICAgICAgICAgICA/ICdiZy1lbWVyYWxkLTk1MC82MCBib3JkZXItZW1lcmFsZC01MDAgdGV4dC1lbWVyYWxkLTIwMCBmb250LWJvbGQnXG4gICAgICAgICAgICAgICAgICAgICAgICA6ICdiZy1zbGF0ZS05MDAgYm9yZGVyLXNsYXRlLTgwMCB0ZXh0LXNsYXRlLTQwMCBob3ZlcjpiZy1zbGF0ZS04MDAnXG4gICAgICAgICAgICAgICAgICAgIH1gfVxuICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHVwcGVyY2FzZSBmb250LW1vbm9cIj5UcnV5IHbhuqVuICN7cS5pZH08L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRydW5jYXRlIHRleHQtd2hpdGVcIj57cS50aXRsZX08L2Rpdj5cbiAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICB7LyogQWN0aXZlIHF1ZXJ5IGRpc3BsYXkgKi99XG4gICAgICAgICAgICAgIHsoKCkgPT4ge1xuICAgICAgICAgICAgICAgIGNvbnN0IGN1ciA9IGRlbW9RdWVyaWVzLmZpbmQoKHEpID0+IHEuaWQgPT09IHNlbGVjdGVkRGVtb1F1ZXJ5KSE7XG4gICAgICAgICAgICAgICAgcmV0dXJuIChcbiAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS0zXCI+XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwNTA5MTJdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQtbGcgcC0zXCI+XG4gICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGp1c3RpZnktYmV0d2VlbiBpdGVtcy1jZW50ZXIgbWItMS41IHRleHQtc2xhdGUtNDAwIHRleHQtWzEwcHhdIGZvbnQtbW9ub1wiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4+Q8OCVSBM4buGTkggUE9TVEdJUyBTUUw6PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgbmF2aWdhdG9yLmNsaXBib2FyZC53cml0ZVRleHQoY3VyLnNxbCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgYWxlcnQoJ8SQw6MgY29weSBjw6J1IHRydXkgduG6pW4gU1FMIScpO1xuICAgICAgICAgICAgICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJob3Zlcjp0ZXh0LWVtZXJhbGQtNDAwIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xXCJcbiAgICAgICAgICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgPENvcHkgY2xhc3NOYW1lPVwidy0zIGgtM1wiIC8+XG4gICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuPlNhbyBjaMOpcDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgIDxwcmUgY2xhc3NOYW1lPVwiZm9udC1tb25vIHRleHQtZW1lcmFsZC00MDAgdGV4dC14cyBvdmVyZmxvdy14LWF1dG8gd2hpdGVzcGFjZS1wcmUtd3JhcCBsZWFkaW5nLXJlbGF4ZWRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgIHtjdXIuc3FsfVxuICAgICAgICAgICAgICAgICAgICAgIDwvcHJlPlxuICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICB7LyogUmVzdWx0cyBTYW1wbGUgKi99XG4gICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQtbGcgcC0zIHNwYWNlLXktMlwiPlxuICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gZm9udC1ib2xkIHVwcGVyY2FzZSB0ZXh0LXNsYXRlLTMwMFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgS+G6v3QgcXXhuqMgbeG6q3UgdHLhuqMgduG7gSB04burIGPGoSBz4bufIGThu68gbGnhu4d1OlxuICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwib3ZlcmZsb3cteC1hdXRvXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8dGFibGUgY2xhc3NOYW1lPVwidy1mdWxsIHRleHQtbGVmdCBmb250LW1vbm8gdGV4dC14cyB0ZXh0LXNsYXRlLTMwMFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICA8dGhlYWQgY2xhc3NOYW1lPVwiYmctWyMwOTBmMWRdIHRleHQtc2xhdGUtNDAwIHRleHQtWzEwcHhdIGJvcmRlci1iIGJvcmRlci1zbGF0ZS04MDBcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dHI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0yXCI+TcOjIC8gVGnDqnUgY2jDrTwvdGg+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0yXCI+VMOqbiDEkcahbiB24buLIC8gQ2hpIHRp4bq/dDwvdGg+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0yXCI+S2hv4bqjbmcgY8OhY2ggLyBHaGkgY2jDujwvdGg+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgPC90cj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgPC90aGVhZD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgPHRib2R5IGNsYXNzTmFtZT1cImRpdmlkZS15IGRpdmlkZS1zbGF0ZS04MDBcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICB7Y3VyLnJlc3VsdFNhbXBsZS5tYXAoKHJvdywgaWR4KSA9PiAoXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dHIga2V5PXtpZHh9IGNsYXNzTmFtZT1cImhvdmVyOmJnLXNsYXRlLTgwMC80MFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0yIGZvbnQtYm9sZCB0ZXh0LXNreS00MDBcIj57cm93Lm1hX2Rvbl92aX08L3RkPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0yIHRleHQtd2hpdGVcIj57cm93LnRlbl9kb25fdml9PC90ZD5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMiB0ZXh0LWVtZXJhbGQtMzAwXCI+e3Jvdy5jdWx5fTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L3RyPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgICAgICAgICAgICAgICA8L3Rib2R5PlxuICAgICAgICAgICAgICAgICAgICAgICAgPC90YWJsZT5cbiAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICApO1xuICAgICAgICAgICAgICB9KSgpfVxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgKX1cblxuICAgICAgICAgIHsvKiBUQUIgMzogRlVMTCBTUUwgRERMICovfVxuICAgICAgICAgIHthY3RpdmVUYWIgPT09ICdzY2hlbWEnICYmIChcbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS0zXCI+XG4gICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBqdXN0aWZ5LWJldHdlZW4gaXRlbXMtY2VudGVyXCI+XG4gICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LXNsYXRlLTQwMFwiPlxuICAgICAgICAgICAgICAgICAgU2NyaXB0IGJhbyBn4buTbSBraOG7n2kgdOG6oW8gYuG6o25nLCB0aGnhur90IGzhuq1wIFNwYXRpYWwgR0lTVCBJbmRleGVzIHbDoCBJTlNFUlQge3VuaXRzLmxlbmd0aH0gYuG6o24gZ2hpXG4gICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZUNvcHl9XG4gICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJweC0zIHB5LTEgcm91bmRlZCBiZy1zbGF0ZS04MDAgaG92ZXI6Ymctc2xhdGUtNzAwIHRleHQtc2xhdGUtMjAwIHRleHQteHMgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTFcIlxuICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgIDxDb3B5IGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgICAgICAgIDxzcGFuPntjb3BpZWQgPyAnxJDDoyBzYW8gY2jDqXAnIDogJ1NhbyBjaMOpcCB0b8OgbiBi4buZJ308L3NwYW4+XG4gICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICA8cHJlIGNsYXNzTmFtZT1cInAtNCBiZy1bIzA1MDkxMl0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC1sZyBmb250LW1vbm8gdGV4dC1lbWVyYWxkLTQwMCB0ZXh0LVsxMXB4XSBtYXgtaC05NiBvdmVyZmxvdy15LWF1dG8gd2hpdGVzcGFjZS1wcmUtd3JhcCBsZWFkaW5nLXJlbGF4ZWRcIj5cbiAgICAgICAgICAgICAgICB7ZnVsbFNRTH1cbiAgICAgICAgICAgICAgPC9wcmU+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICApfVxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogRm9vdGVyICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInAtNCBiZy1zbGF0ZS05MDAgYm9yZGVyLXQgYm9yZGVyLXNsYXRlLTgwMCBmbGV4IGp1c3RpZnktZW5kXCI+XG4gICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgb25DbGljaz17b25DbG9zZX1cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cInB4LTQgcHktMiByb3VuZGVkIGJnLXNsYXRlLTgwMCBob3ZlcjpiZy1zbGF0ZS03MDAgdGV4dC13aGl0ZSBmb250LW1lZGl1bSB0ZXh0LXhzXCJcbiAgICAgICAgICA+XG4gICAgICAgICAgICDEkMOzbmcgY+G7rWEgc+G7lVxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8L2Rpdj5cbiAgICAgIDwvZGl2PlxuICAgIDwvZGl2PlxuICApO1xufTtcbiJdfQ==