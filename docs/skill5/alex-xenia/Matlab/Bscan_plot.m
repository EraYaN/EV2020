function H_out = Bscan_plot( t_smp , x_smp , Data , ...
                             x_label , y_label , z_label , ...
                             Orientation );

% ---------------------------------------------------------------------
%
%     Draws a B-scan plot based on a time-x-value 2D matrix.
%
%     The significance of the parameters:
%
%     Input:
%
%     - t_smp ( real() )
%        the time samples - mandatory parameter (the length of the
%        vector is M);
%
%     - x_smp ( real() )
%        the space samples - mandatory parameter (the length of the
%        vector is N);
%
%     - Data ( real(M,N) )
%        the values to be plotted - mandatory parameter;
%
%     - x_label , y_label , z_label ( string )
%        the labels used for the x-, y- and z- axes - mandatory
%        parameter;
%
%     - Orientation ( real(2) )
%        the angles used in the view standard Matlab function -
%        optional parameter, by default [ 20 45 ];
%
%     Output:
%
%     - H_out ( handle )
%        the handle to the generated plot.
%
% .....................................................................
%
%  NOTE:
%
%     Note...
%
% .....................................................................
%
%  v 1.0 26.03.2011
%
% ---------------------------------------------------------------------

   if ( nargin < 7 )
      Orientation = [ 20 45 ];
   end

   % Set the plotting parameters
   %
   Line_width = 1/3;
   FontSize = 16;
   NotationFontSize = 14;


   % Determine the range of the data to be represented; the range is always
   %  symmetric
   %
   D_max = max( [ max( Data(:) ) abs( min( Data(:) ) ) ] );
   Range = [ -D_max D_max ];

   % Assemble a symmetric (fine) colorbar
   %
   H_col = Rainbow_cols(2);
   L_col = Rainbow_cols(32);
   Z_col = Rainbow_cols(24);  % colour plot
   Z_col = 0.6*[ 1 1 1 ];     % black and white plot
   % Z_col = Rainbow_cols(25);
   %
   Dim_col = 201;
   Dim_c_2 = ceil(Dim_col/2);
   %
   L_seg = [ linspace( L_col(1) , Z_col(1) , Dim_c_2 ) ; ...
             linspace( L_col(2) , Z_col(2) , Dim_c_2 ) ; ...
             linspace( L_col(3) , Z_col(3) , Dim_c_2 ) ];
   %
   H_seg = [ linspace( Z_col(1) , H_col(1) , Dim_c_2 ) ; ...
             linspace( Z_col(2) , H_col(2) , Dim_c_2 ) ; ...
             linspace( Z_col(3) , H_col(3) , Dim_c_2 ) ];
   %
   Col_map = [ L_seg(1,:) H_seg(1,2:end) ; ...
               L_seg(2,:) H_seg(2,2:end) ; ...
               L_seg(3,:) H_seg(3,2:end) ];
   %
   Col_map = Col_map';
   % Col_map = [ [1 1 1] ; Col_map' ];


   % ииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииии
   %
   %                 GENERATE THE PLOT
   %
   % ииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииииии

   figure(1)
   clf;
   axis( [ t_smp(1) t_smp(end) x_smp(1) x_smp(end) -1.05*D_max 1.05*D_max ] );
   %
   hold on
   hw = waterfall( t_smp , x_smp , Data );
   set( hw , 'LineWidth' , Line_width );
   hold off

   % Delete the extra lines at the "waterfall"
   %
   Vertices = get(hw,'Vertices');
   FVCData = get(hw,'FaceVertexCData');
   %
   t_eps = 1e-6*( t_smp(2) - t_smp(1) );
   i_NaN = [];
   %
   %  - check the t_smp(1) end
   %
   NaN_flag = 0;
   for ii=1:size(Vertices)
      fvc = FVCData(ii);
      vert = Vertices(ii,:);
      %
      if ( abs(vert(1)-t_smp(1)) < t_eps )
         if ~NaN_flag
            i_NaN = [ i_NaN ii ];
            NaN_flag = 1;
         else
            NaN_flag = 1;
         end
      else
         NaN_flag = 0;
      end
   end
   %
   %  - check the t_smp(end) end
   %
   NaN_flag = 0;
   for ii=1:size(Vertices)
      fvc = FVCData(ii);
      vert = Vertices(ii,:);
      %
      if ( abs(vert(1)-t_smp(end)) < t_eps )
         if ~NaN_flag
            NaN_flag = 0.5;
         elseif ( abs(NaN_flag-0.5) < 1e-6 )
            i_NaN = [ i_NaN ii ];
            NaN_flag = 1;
         else
            NaN_flag = 1;
         end
      else
         NaN_flag = 0;
      end
   end
   %
   FVCData(i_NaN) = NaN; set(hw,'FaceVertexCData',FVCData);

   % Set the representation parameters
   %
   view( Orientation(1) , Orientation(2) );
   set( gca , 'Layer' , 'top' , ...
              'FontSize' , NotationFontSize );
   %
   box on;
   grid on;
   %
   caxis( [-D_max D_max] );
   colormap(Col_map);
   % colorbar

   % Add the labels
   %
   xlabel( x_label , 'FontSize' , FontSize , 'Interpreter' , 'LaTeX' );
   ylabel( y_label , 'FontSize' , FontSize , 'Interpreter' , 'LaTeX' );
   zlabel( z_label , 'FontSize' , FontSize , 'Interpreter' , 'LaTeX' );

return