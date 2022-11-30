import { Map } from "./components/Map";
import { VendorTable } from "./components/VendorTable/VendorTable";
import { Home } from "./components/Home";
import { Analytics } from "./components/Analytics";


const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/map',
    element: <Map />
  },
  {
    path: '/analytics',
    element: <Analytics />
  },
  {
    path: '/vendor-table',
    element: <VendorTable />
  }
];

export default AppRoutes;
