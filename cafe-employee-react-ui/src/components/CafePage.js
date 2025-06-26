import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AgGridReact } from 'ag-grid-react';
import {
  Box, Button, Typography, IconButton, Tooltip,
  Snackbar, Alert, TextField
} from '@mui/material';

import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

import ConfirmDialog from './common/ConfirmDialog';
import CafeDialog from './common/CafeDialog';
import NavigationTabs from './common/NavigationTabs';

import { provideGlobalGridOptions } from 'ag-grid-community';
import { getCafes, deleteCafe } from '../services/api';

import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-material.css';

// Register AG Grid modules
ModuleRegistry.registerModules([AllCommunityModule]);

// Ensure legacy themes work
provideGlobalGridOptions({ theme: "legacy" });

function CafePage() {
  // State declarations
  const [cafes, setCafes] = useState([]);
  const [confirmDialog, setConfirmDialog] = useState(false);
  const [selectedCafe, setSelectedCafe] = useState(null);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [searchValue, setSearchValue] = useState('');
  const navigate = useNavigate();

  // Load cafes on component load
  useEffect(() => {
    fetchCafes();
  }, []);

  // Fetch all cafes from API
  const fetchCafes = async () => {
    try {
      const response = await getCafes();
      setCafes(response.data);
    } catch (error) {
      console.error('Failed to fetch cafes:', error);
    }
  };

  // Delete a cafe after confirmation
  const handleDelete = async () => {
    try {
      await deleteCafe(selectedCafe.id);
      setCafes((prev) => prev.filter(cafe => cafe.id !== selectedCafe.id));
      setConfirmDialog(false);
      showSnackbar('Cafe details deleted successfully', 'success');
    } catch (error) {
      console.error('Failed to delete cafe:', error);
    }
  };

  // Open dialog to edit or create a cafe
  const handleEditClick = (cafe = null) => {
    setSelectedCafe(cafe || {});
    setEditDialogOpen(true);
  };

  // After save, refresh list and show message
  const handleSave = () => {
    setEditDialogOpen(false);
    fetchCafes();
    showSnackbar(
      selectedCafe.id
        ? 'Cafe details updated successfully'
        : 'New Cafe added successfully',
      'success'
    );
  };

  // Reusable snackbar helper
  const showSnackbar = (message, severity) => {
    setSnackbar({ open: true, message, severity });
  };

  // Filter cafes by location
  const handleSearch = (value) => {
    const lowerSearch = value.toLowerCase().trim();
    if (lowerSearch) {
      const filtered = cafes.filter(cf =>
        cf.location?.toLowerCase().includes(lowerSearch)
      );
      setCafes(filtered);
    } else {
      fetchCafes(); // Reset if empty
    }
  };

  // AG Grid column definitions
  const _columnDefs = [
    {
      headerName: 'Logo',
      field: 'logo',
      width: 120,
      cellRenderer: (params) =>
        params.value ? <img src={params.value} alt="logo" style={{ height: 40 }} /> : '',
    },
    { headerName: 'Name', field: 'name' },
    { headerName: 'Description', field: 'description' },
    { headerName: 'Location', field: 'location' },
    {
      headerName: 'Employees',
      field: 'employeesCount',
      cellRenderer: (params) => (
        <span
          style={{ color: '#1976d2', cursor: 'pointer', textDecoration: 'underline' }}
          onClick={() => navigate(`/employees?cafeId=${params.data.id}`)}
        >
          {params.data.employeesCount ?? 0}
        </span>
      ),
    },
    {
      headerName: 'Actions',
      cellRenderer: (params) => (
        <Box>
          <Tooltip title="Edit">
            <IconButton
              size="small"
              onClick={() => handleEditClick(params.data)}
              sx={{ mr: 1 }}
              aria-label="Edit"
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete">
            <IconButton
              size="small"
              color="error"
              onClick={() => {
                setSelectedCafe(params.data);
                setConfirmDialog(true);
              }}
              aria-label="Delete"
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
      ),
    },
  ];

  return (
    <Box>
      {/* Tabs for navigation between Cafes / Employees */}
      <NavigationTabs />

      {/* Page Title */}
      <Typography variant="h4" gutterBottom>Cafes</Typography>

      {/* Search and Add Controls */}
      <Box display="flex" justifyContent="space-between" alignItems="center" gap={2} mb={2}>
        <TextField
          name="location"
          label="Search Cafe By Location"
          value={searchValue}
          onChange={(e) => {
            const value = e.target.value;
            setSearchValue(value);
            handleSearch(value);
          }}
          margin="normal"
          size="small"
        />
        <Button
          variant="contained"
          color="primary"
          onClick={handleEditClick}
          sx={{ mb: 2 }}
        >
          Add New Cafe
        </Button>
      </Box>

      {/* AG Grid for Cafe Listing */}
      <div className="ag-theme-material" style={{ height: 400 }}>
        <AgGridReact rowData={cafes} columnDefs={_columnDefs} pagination />
      </div>

      {/* Confirm Delete Dialog */}
      <ConfirmDialog
        open={confirmDialog}
        onClose={() => setConfirmDialog(false)}
        onConfirm={handleDelete}
        title="Delete Cafe"
        message="Are you sure you want to delete this cafe?"
      />

      {/* Add/Edit Cafe Dialog */}
      <CafeDialog
        open={editDialogOpen}
        formData={selectedCafe}
        onClose={() => setEditDialogOpen(false)}
        onSave={handleSave}
      />

      {/* Snackbar for success messages */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={2000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setSnackbar({ ...snackbar, open: false })}
          severity={snackbar.severity}
          sx={{ width: '100%' }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}

export default CafePage;
