function errors = location_errors()
    STEPS = 50;
    errors = zeros(STEPS-1,STEPS-1);
    errors_vector = zeros((STEPS-1)*(STEPS-1),1);

    c = 340;
    h = 0.28; % 28 cm from the ground
    mics = [0 0 0.38; 0 2.90 0.38; 2.90 2.90 0.38; 2.90 0 0.38; -1.05 1.45 0.93];
    
    for test_x_index = 1:(STEPS-1)
        test_x = test_x_index / STEPS * 2.9;
        
        for test_y_index = 1:(STEPS-1)
            test_y = test_y_index / STEPS * 2.9;
            
            loc = [test_x+0.000001 test_y+0.000001 h] % Prevent NaN as resulting loc
            % Calculate the times that it would take to reach the
            % microphones from the beacon
            t = [];
            for i = 1:size(mics, 1)
                t(i) = sqrt(sum((mics(i,:) - loc).^2)) / c + 0.1; % Add some offset to test offset elimination too
            end
            
            loc = location(t);
            errors(test_x_index, test_y_index) = sqrt(sum((loc - [test_x,test_y]).^2));
            errors_vector(test_x_index*(STEPS-1) + test_y_index) = sqrt(sum((loc - [test_x,test_y]).^2));
        end
    end
    
    error_mean = mean(errors_vector);
    error_std = std(errors_vector);
    error_max = max(errors_vector);
    
    sprintf('Mean: %.6f Std: %.6f Max: %.6f', error_mean, error_std, error_max)
end