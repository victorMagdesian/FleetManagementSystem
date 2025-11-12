export interface Driver {
  id: string;
  name: string;
  licenseNumber: string;
  phone: string;
  active: boolean;
}

export interface CreateDriverRequest {
  name: string;
  licenseNumber: string;
  phone: string;
}

export interface UpdateDriverRequest {
  name: string;
  licenseNumber: string;
  phone: string;
}
