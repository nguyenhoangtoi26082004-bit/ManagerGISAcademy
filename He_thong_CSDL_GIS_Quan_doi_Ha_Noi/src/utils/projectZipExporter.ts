const JSZip = __vite__cjsImport0_jszip;import __vite__cjsImport0_jszip from "/node_modules/.vite/deps/jszip.js?v=de843665";
import { generatePostgisSQL, unitsToCSV } from "/src/utils/postgisExport.ts";
import { unitsToGeoJSON } from "/src/utils/gisCalculations.ts";
export async function exportFullProjectZip(units) {
	const zip = new JSZip();
	const fileList = [
		"package.json",
		"tsconfig.json",
		"vite.config.ts",
		"index.html",
		"metadata.json",
		".env.example",
		"README.md",
		"src/main.tsx",
		"src/App.tsx",
		"src/index.css",
		"src/types/gis.ts",
		"src/data/hanoiDistricts.ts",
		"src/data/militaryUnits.ts",
		"src/utils/coordinateConversion.ts",
		"src/utils/gisCalculations.ts",
		"src/utils/postgisExport.ts",
		"src/utils/projectZipExporter.ts",
		"src/components/Header.tsx",
		"src/components/Sidebar.tsx",
		"src/components/StatusBar.tsx",
		"src/components/GisMap.tsx",
		"src/components/UnitDetailDrawer.tsx",
		"src/components/DashboardView.tsx",
		"src/components/UnitSearchView.tsx",
		"src/components/DataManagementView.tsx",
		"src/components/SpatialAnalysisView.tsx",
		"src/components/StatisticsView.tsx",
		"src/components/DataCheckerView.tsx",
		"src/components/UnitModal.tsx",
		"src/components/PostgisModal.tsx",
		"docs/01_WINFORMS_MAPWINGIS_STRUCTURE.md",
		"docs/02_POSTGIS_DATABASE_SYNC.md",
		"docs/03_BACKEND_ARCHITECTURE.md",
		"docs/04_PROJECT_ROADMAP_AND_FUNCTIONAL_DESIGN.md",
		"docs/05_GITHUB_COLLABORATION_AND_TEAMWORK.md",
		"docs/06_REACT_TO_WINFORMS_MAPWINGIS_MAPPING_GUIDE.md",
		"docs/07_HUONG_DAN_NAP_DU_LIEU_VAO_WINFORMS_MAPWINGIS.md"
	];
	// Fetch all source files from dev server
	await Promise.all(fileList.map(async (filePath) => {
		try {
			const response = await fetch(`/${filePath}`);
			if (response.ok) {
				const text = await response.text();
				zip.file(filePath, text);
			}
		} catch (err) {
			console.warn(`Could not fetch ${filePath}, using fallback`, err);
		}
	}));
	// Generate C# Seed Data code
	let csSeedData = `using System.Collections.Generic;

namespace HanoiMilitaryGIS.DTO
{
    public static class DonViQuanDoiSeedData
    {
        public static List<DonViQuanDoi> LayDanhSachDonViMau()
        {
            return new List<DonViQuanDoi>
            {
`;
	units.forEach((u) => {
		const missions = u.mainMissions.map((m) => `"${m.replace(/"/g, "\"\"")}"`).join(", ");
		const equips = u.equipmentHighlights.map((e) => `"${e.replace(/"/g, "\"\"")}"`).join(", ");
		csSeedData += `                new DonViQuanDoi
                {
                    MaDonVi = "${u.code}",
                    TenDonVi = "${u.name.replace(/"/g, "\"\"")}",
                    TenVietTat = "${u.shortName.replace(/"/g, "\"\"")}",
                    LoaiDonVi = "${u.unitType}",
                    QuanBinhChung = "${u.branch}",
                    DonViCapTren = "${u.parentUnit.replace(/"/g, "\"\"")}",
                    QuanHuyen = "${u.district}",
                    DiaChi = "${u.address.replace(/"/g, "\"\"")}",
                    ViDo = ${u.latitude},
                    KinhDo = ${u.longitude},
                    XVn2000 = ${u.x_vn2000},
                    YVn2000 = ${u.y_vn2000},
                    ChiHuyTruong = "${(u.commander || "").replace(/"/g, "\"\"")}",
                    SoDienThoai = "${u.phone}",
                    TrangThaiSSCD = "${u.readinessStatus}",
                    DoMat = "${u.securityLevel}",
                    NhiemVuChinh = new string[] { ${missions} },
                    VuKhiTrangBi = new string[] { ${equips} }
                },
`;
	});
	csSeedData += `            };
        }
    }
}
`;
	// Ensure database directory has pre-exported files
	const dbFolder = zip.folder("database");
	if (dbFolder) {
		dbFolder.file("hanoi_military_gis_postgis.sql", generatePostgisSQL(units));
		dbFolder.file("hanoi_military_units.geojson", unitsToGeoJSON(units));
		dbFolder.file("hanoi_military_units.csv", unitsToCSV(units));
		dbFolder.file("DonViQuanDoiSeedData.cs", csSeedData);
	}
	// Generate the zip blob
	const content = await zip.generateAsync({ type: "blob" });
	// Trigger download
	const url = URL.createObjectURL(content);
	const a = document.createElement("a");
	a.href = url;
	a.download = "He_thong_CSDL_GIS_Quan_doi_Ha_Noi.zip";
	document.body.appendChild(a);
	a.click();
	document.body.removeChild(a);
	URL.revokeObjectURL(url);
}

//# sourceMappingURL=data:application/json;base64,eyJtYXBwaW5ncyI6IkFBQUEsT0FBTyxXQUFXO0FBRWxCLFNBQVMsb0JBQW9CLGtCQUFrQjtBQUMvQyxTQUFTLHNCQUFzQjtBQUUvQixPQUFPLGVBQWUscUJBQXFCLE9BQXNDO0NBQy9FLE1BQU0sTUFBTSxJQUFJLE1BQU07Q0FFdEIsTUFBTSxXQUFXO0VBQ2Y7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7RUFDQTtFQUNBO0VBQ0E7Q0FDRjs7Q0FHQSxNQUFNLFFBQVEsSUFDWixTQUFTLElBQUksT0FBTyxhQUFhO0VBQy9CLElBQUk7R0FDRixNQUFNLFdBQVcsTUFBTSxNQUFNLElBQUksVUFBVTtHQUMzQyxJQUFJLFNBQVMsSUFBSTtJQUNmLE1BQU0sT0FBTyxNQUFNLFNBQVMsS0FBSztJQUNqQyxJQUFJLEtBQUssVUFBVSxJQUFJO0dBQ3pCO0VBQ0YsU0FBUyxLQUFLO0dBQ1osUUFBUSxLQUFLLG1CQUFtQixTQUFTLG1CQUFtQixHQUFHO0VBQ2pFO0NBQ0YsQ0FBQyxDQUNIOztDQUdBLElBQUksYUFBYTs7Ozs7Ozs7Ozs7Q0FXakIsTUFBTSxTQUFTLE1BQU07RUFDbkIsTUFBTSxXQUFXLEVBQUUsYUFBYSxLQUFLLE1BQU0sSUFBSSxFQUFFLFFBQVEsTUFBTSxNQUFJLEVBQUUsRUFBRSxDQUFDLENBQUMsS0FBSyxJQUFJO0VBQ2xGLE1BQU0sU0FBUyxFQUFFLG9CQUFvQixLQUFLLE1BQU0sSUFBSSxFQUFFLFFBQVEsTUFBTSxNQUFJLEVBQUUsRUFBRSxDQUFDLENBQUMsS0FBSyxJQUFJO0VBQ3ZGLGNBQWM7O2lDQUVlLEVBQUUsS0FBSztrQ0FDTixFQUFFLEtBQUssUUFBUSxNQUFNLE1BQUksRUFBRTtvQ0FDekIsRUFBRSxVQUFVLFFBQVEsTUFBTSxNQUFJLEVBQUU7bUNBQ2pDLEVBQUUsU0FBUzt1Q0FDUCxFQUFFLE9BQU87c0NBQ1YsRUFBRSxXQUFXLFFBQVEsTUFBTSxNQUFJLEVBQUU7bUNBQ3BDLEVBQUUsU0FBUztnQ0FDZCxFQUFFLFFBQVEsUUFBUSxNQUFNLE1BQUksRUFBRTs2QkFDakMsRUFBRSxTQUFTOytCQUNULEVBQUUsVUFBVTtnQ0FDWCxFQUFFLFNBQVM7Z0NBQ1gsRUFBRSxTQUFTO3VDQUNKLEVBQUUsYUFBYSxHQUFFLENBQUUsUUFBUSxNQUFNLE1BQUksRUFBRTtxQ0FDekMsRUFBRSxNQUFNO3VDQUNOLEVBQUUsZ0JBQWdCOytCQUMxQixFQUFFLGNBQWM7b0RBQ0ssU0FBUztvREFDVCxPQUFPOzs7Q0FHekQsQ0FBQztDQUNELGNBQWM7Ozs7OztDQU9kLE1BQU0sV0FBVyxJQUFJLE9BQU8sVUFBVTtDQUN0QyxJQUFJLFVBQVU7RUFDWixTQUFTLEtBQUssa0NBQWtDLG1CQUFtQixLQUFLLENBQUM7RUFDekUsU0FBUyxLQUFLLGdDQUFnQyxlQUFlLEtBQUssQ0FBQztFQUNuRSxTQUFTLEtBQUssNEJBQTRCLFdBQVcsS0FBSyxDQUFDO0VBQzNELFNBQVMsS0FBSywyQkFBMkIsVUFBVTtDQUNyRDs7Q0FHQSxNQUFNLFVBQVUsTUFBTSxJQUFJLGNBQWMsRUFBRSxNQUFNLE9BQU8sQ0FBQzs7Q0FHeEQsTUFBTSxNQUFNLElBQUksZ0JBQWdCLE9BQU87Q0FDdkMsTUFBTSxJQUFJLFNBQVMsY0FBYyxHQUFHO0NBQ3BDLEVBQUUsT0FBTztDQUNULEVBQUUsV0FBVztDQUNiLFNBQVMsS0FBSyxZQUFZLENBQUM7Q0FDM0IsRUFBRSxNQUFNO0NBQ1IsU0FBUyxLQUFLLFlBQVksQ0FBQztDQUMzQixJQUFJLGdCQUFnQixHQUFHO0FBQ3pCIiwibmFtZXMiOltdLCJzb3VyY2VzIjpbInByb2plY3RaaXBFeHBvcnRlci50cyJdLCJ2ZXJzaW9uIjozLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgSlNaaXAgZnJvbSAnanN6aXAnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0IH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IGdlbmVyYXRlUG9zdGdpc1NRTCwgdW5pdHNUb0NTViB9IGZyb20gJy4vcG9zdGdpc0V4cG9ydCc7XG5pbXBvcnQgeyB1bml0c1RvR2VvSlNPTiB9IGZyb20gJy4vZ2lzQ2FsY3VsYXRpb25zJztcblxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGV4cG9ydEZ1bGxQcm9qZWN0WmlwKHVuaXRzOiBNaWxpdGFyeVVuaXRbXSk6IFByb21pc2U8dm9pZD4ge1xuICBjb25zdCB6aXAgPSBuZXcgSlNaaXAoKTtcblxuICBjb25zdCBmaWxlTGlzdCA9IFtcbiAgICAncGFja2FnZS5qc29uJyxcbiAgICAndHNjb25maWcuanNvbicsXG4gICAgJ3ZpdGUuY29uZmlnLnRzJyxcbiAgICAnaW5kZXguaHRtbCcsXG4gICAgJ21ldGFkYXRhLmpzb24nLFxuICAgICcuZW52LmV4YW1wbGUnLFxuICAgICdSRUFETUUubWQnLFxuICAgICdzcmMvbWFpbi50c3gnLFxuICAgICdzcmMvQXBwLnRzeCcsXG4gICAgJ3NyYy9pbmRleC5jc3MnLFxuICAgICdzcmMvdHlwZXMvZ2lzLnRzJyxcbiAgICAnc3JjL2RhdGEvaGFub2lEaXN0cmljdHMudHMnLFxuICAgICdzcmMvZGF0YS9taWxpdGFyeVVuaXRzLnRzJyxcbiAgICAnc3JjL3V0aWxzL2Nvb3JkaW5hdGVDb252ZXJzaW9uLnRzJyxcbiAgICAnc3JjL3V0aWxzL2dpc0NhbGN1bGF0aW9ucy50cycsXG4gICAgJ3NyYy91dGlscy9wb3N0Z2lzRXhwb3J0LnRzJyxcbiAgICAnc3JjL3V0aWxzL3Byb2plY3RaaXBFeHBvcnRlci50cycsXG4gICAgJ3NyYy9jb21wb25lbnRzL0hlYWRlci50c3gnLFxuICAgICdzcmMvY29tcG9uZW50cy9TaWRlYmFyLnRzeCcsXG4gICAgJ3NyYy9jb21wb25lbnRzL1N0YXR1c0Jhci50c3gnLFxuICAgICdzcmMvY29tcG9uZW50cy9HaXNNYXAudHN4JyxcbiAgICAnc3JjL2NvbXBvbmVudHMvVW5pdERldGFpbERyYXdlci50c3gnLFxuICAgICdzcmMvY29tcG9uZW50cy9EYXNoYm9hcmRWaWV3LnRzeCcsXG4gICAgJ3NyYy9jb21wb25lbnRzL1VuaXRTZWFyY2hWaWV3LnRzeCcsXG4gICAgJ3NyYy9jb21wb25lbnRzL0RhdGFNYW5hZ2VtZW50Vmlldy50c3gnLFxuICAgICdzcmMvY29tcG9uZW50cy9TcGF0aWFsQW5hbHlzaXNWaWV3LnRzeCcsXG4gICAgJ3NyYy9jb21wb25lbnRzL1N0YXRpc3RpY3NWaWV3LnRzeCcsXG4gICAgJ3NyYy9jb21wb25lbnRzL0RhdGFDaGVja2VyVmlldy50c3gnLFxuICAgICdzcmMvY29tcG9uZW50cy9Vbml0TW9kYWwudHN4JyxcbiAgICAnc3JjL2NvbXBvbmVudHMvUG9zdGdpc01vZGFsLnRzeCcsXG4gICAgJ2RvY3MvMDFfV0lORk9STVNfTUFQV0lOR0lTX1NUUlVDVFVSRS5tZCcsXG4gICAgJ2RvY3MvMDJfUE9TVEdJU19EQVRBQkFTRV9TWU5DLm1kJyxcbiAgICAnZG9jcy8wM19CQUNLRU5EX0FSQ0hJVEVDVFVSRS5tZCcsXG4gICAgJ2RvY3MvMDRfUFJPSkVDVF9ST0FETUFQX0FORF9GVU5DVElPTkFMX0RFU0lHTi5tZCcsXG4gICAgJ2RvY3MvMDVfR0lUSFVCX0NPTExBQk9SQVRJT05fQU5EX1RFQU1XT1JLLm1kJyxcbiAgICAnZG9jcy8wNl9SRUFDVF9UT19XSU5GT1JNU19NQVBXSU5HSVNfTUFQUElOR19HVUlERS5tZCcsXG4gICAgJ2RvY3MvMDdfSFVPTkdfREFOX05BUF9EVV9MSUVVX1ZBT19XSU5GT1JNU19NQVBXSU5HSVMubWQnLFxuICBdO1xuXG4gIC8vIEZldGNoIGFsbCBzb3VyY2UgZmlsZXMgZnJvbSBkZXYgc2VydmVyXG4gIGF3YWl0IFByb21pc2UuYWxsKFxuICAgIGZpbGVMaXN0Lm1hcChhc3luYyAoZmlsZVBhdGgpID0+IHtcbiAgICAgIHRyeSB7XG4gICAgICAgIGNvbnN0IHJlc3BvbnNlID0gYXdhaXQgZmV0Y2goYC8ke2ZpbGVQYXRofWApO1xuICAgICAgICBpZiAocmVzcG9uc2Uub2spIHtcbiAgICAgICAgICBjb25zdCB0ZXh0ID0gYXdhaXQgcmVzcG9uc2UudGV4dCgpO1xuICAgICAgICAgIHppcC5maWxlKGZpbGVQYXRoLCB0ZXh0KTtcbiAgICAgICAgfVxuICAgICAgfSBjYXRjaCAoZXJyKSB7XG4gICAgICAgIGNvbnNvbGUud2FybihgQ291bGQgbm90IGZldGNoICR7ZmlsZVBhdGh9LCB1c2luZyBmYWxsYmFja2AsIGVycik7XG4gICAgICB9XG4gICAgfSlcbiAgKTtcblxuICAvLyBHZW5lcmF0ZSBDIyBTZWVkIERhdGEgY29kZVxuICBsZXQgY3NTZWVkRGF0YSA9IGB1c2luZyBTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYztcblxubmFtZXNwYWNlIEhhbm9pTWlsaXRhcnlHSVMuRFRPXG57XG4gICAgcHVibGljIHN0YXRpYyBjbGFzcyBEb25WaVF1YW5Eb2lTZWVkRGF0YVxuICAgIHtcbiAgICAgICAgcHVibGljIHN0YXRpYyBMaXN0PERvblZpUXVhbkRvaT4gTGF5RGFuaFNhY2hEb25WaU1hdSgpXG4gICAgICAgIHtcbiAgICAgICAgICAgIHJldHVybiBuZXcgTGlzdDxEb25WaVF1YW5Eb2k+XG4gICAgICAgICAgICB7XG5gO1xuICB1bml0cy5mb3JFYWNoKCh1KSA9PiB7XG4gICAgY29uc3QgbWlzc2lvbnMgPSB1Lm1haW5NaXNzaW9ucy5tYXAoKG0pID0+IGBcIiR7bS5yZXBsYWNlKC9cIi9nLCAnXCJcIicpfVwiYCkuam9pbignLCAnKTtcbiAgICBjb25zdCBlcXVpcHMgPSB1LmVxdWlwbWVudEhpZ2hsaWdodHMubWFwKChlKSA9PiBgXCIke2UucmVwbGFjZSgvXCIvZywgJ1wiXCInKX1cImApLmpvaW4oJywgJyk7XG4gICAgY3NTZWVkRGF0YSArPSBgICAgICAgICAgICAgICAgIG5ldyBEb25WaVF1YW5Eb2lcbiAgICAgICAgICAgICAgICB7XG4gICAgICAgICAgICAgICAgICAgIE1hRG9uVmkgPSBcIiR7dS5jb2RlfVwiLFxuICAgICAgICAgICAgICAgICAgICBUZW5Eb25WaSA9IFwiJHt1Lm5hbWUucmVwbGFjZSgvXCIvZywgJ1wiXCInKX1cIixcbiAgICAgICAgICAgICAgICAgICAgVGVuVmlldFRhdCA9IFwiJHt1LnNob3J0TmFtZS5yZXBsYWNlKC9cIi9nLCAnXCJcIicpfVwiLFxuICAgICAgICAgICAgICAgICAgICBMb2FpRG9uVmkgPSBcIiR7dS51bml0VHlwZX1cIixcbiAgICAgICAgICAgICAgICAgICAgUXVhbkJpbmhDaHVuZyA9IFwiJHt1LmJyYW5jaH1cIixcbiAgICAgICAgICAgICAgICAgICAgRG9uVmlDYXBUcmVuID0gXCIke3UucGFyZW50VW5pdC5yZXBsYWNlKC9cIi9nLCAnXCJcIicpfVwiLFxuICAgICAgICAgICAgICAgICAgICBRdWFuSHV5ZW4gPSBcIiR7dS5kaXN0cmljdH1cIixcbiAgICAgICAgICAgICAgICAgICAgRGlhQ2hpID0gXCIke3UuYWRkcmVzcy5yZXBsYWNlKC9cIi9nLCAnXCJcIicpfVwiLFxuICAgICAgICAgICAgICAgICAgICBWaURvID0gJHt1LmxhdGl0dWRlfSxcbiAgICAgICAgICAgICAgICAgICAgS2luaERvID0gJHt1LmxvbmdpdHVkZX0sXG4gICAgICAgICAgICAgICAgICAgIFhWbjIwMDAgPSAke3UueF92bjIwMDB9LFxuICAgICAgICAgICAgICAgICAgICBZVm4yMDAwID0gJHt1Lnlfdm4yMDAwfSxcbiAgICAgICAgICAgICAgICAgICAgQ2hpSHV5VHJ1b25nID0gXCIkeyh1LmNvbW1hbmRlciB8fCAnJykucmVwbGFjZSgvXCIvZywgJ1wiXCInKX1cIixcbiAgICAgICAgICAgICAgICAgICAgU29EaWVuVGhvYWkgPSBcIiR7dS5waG9uZX1cIixcbiAgICAgICAgICAgICAgICAgICAgVHJhbmdUaGFpU1NDRCA9IFwiJHt1LnJlYWRpbmVzc1N0YXR1c31cIixcbiAgICAgICAgICAgICAgICAgICAgRG9NYXQgPSBcIiR7dS5zZWN1cml0eUxldmVsfVwiLFxuICAgICAgICAgICAgICAgICAgICBOaGllbVZ1Q2hpbmggPSBuZXcgc3RyaW5nW10geyAke21pc3Npb25zfSB9LFxuICAgICAgICAgICAgICAgICAgICBWdUtoaVRyYW5nQmkgPSBuZXcgc3RyaW5nW10geyAke2VxdWlwc30gfVxuICAgICAgICAgICAgICAgIH0sXG5gO1xuICB9KTtcbiAgY3NTZWVkRGF0YSArPSBgICAgICAgICAgICAgfTtcbiAgICAgICAgfVxuICAgIH1cbn1cbmA7XG5cbiAgLy8gRW5zdXJlIGRhdGFiYXNlIGRpcmVjdG9yeSBoYXMgcHJlLWV4cG9ydGVkIGZpbGVzXG4gIGNvbnN0IGRiRm9sZGVyID0gemlwLmZvbGRlcignZGF0YWJhc2UnKTtcbiAgaWYgKGRiRm9sZGVyKSB7XG4gICAgZGJGb2xkZXIuZmlsZSgnaGFub2lfbWlsaXRhcnlfZ2lzX3Bvc3RnaXMuc3FsJywgZ2VuZXJhdGVQb3N0Z2lzU1FMKHVuaXRzKSk7XG4gICAgZGJGb2xkZXIuZmlsZSgnaGFub2lfbWlsaXRhcnlfdW5pdHMuZ2VvanNvbicsIHVuaXRzVG9HZW9KU09OKHVuaXRzKSk7XG4gICAgZGJGb2xkZXIuZmlsZSgnaGFub2lfbWlsaXRhcnlfdW5pdHMuY3N2JywgdW5pdHNUb0NTVih1bml0cykpO1xuICAgIGRiRm9sZGVyLmZpbGUoJ0RvblZpUXVhbkRvaVNlZWREYXRhLmNzJywgY3NTZWVkRGF0YSk7XG4gIH1cblxuICAvLyBHZW5lcmF0ZSB0aGUgemlwIGJsb2JcbiAgY29uc3QgY29udGVudCA9IGF3YWl0IHppcC5nZW5lcmF0ZUFzeW5jKHsgdHlwZTogJ2Jsb2InIH0pO1xuXG4gIC8vIFRyaWdnZXIgZG93bmxvYWRcbiAgY29uc3QgdXJsID0gVVJMLmNyZWF0ZU9iamVjdFVSTChjb250ZW50KTtcbiAgY29uc3QgYSA9IGRvY3VtZW50LmNyZWF0ZUVsZW1lbnQoJ2EnKTtcbiAgYS5ocmVmID0gdXJsO1xuICBhLmRvd25sb2FkID0gJ0hlX3Rob25nX0NTRExfR0lTX1F1YW5fZG9pX0hhX05vaS56aXAnO1xuICBkb2N1bWVudC5ib2R5LmFwcGVuZENoaWxkKGEpO1xuICBhLmNsaWNrKCk7XG4gIGRvY3VtZW50LmJvZHkucmVtb3ZlQ2hpbGQoYSk7XG4gIFVSTC5yZXZva2VPYmplY3RVUkwodXJsKTtcbn1cbiJdfQ==