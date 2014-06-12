% X: toepitz matrix
% y: N vectors with output data from microphones
% Fs_RX: 48000 Hz in our case
function t = matched_filter_toep(X, y, Fs_RX)
    
    if length(size(y)) == 3
       y = squeeze(y); % 3D to 2D matrix
    end
    
    t = zeros(size(y,2),1);
    for i = 1:size(y,2)
        % Calculate the matched filter result for this mic channel
        [hi,t(i)] = single_matched_filter(X, y(:,i));
        
        % Sample number to time
        t(i) = t(i)/Fs_RX;
        
        subplot(size(y,2),2,i*2-1);
        LinePlotReducer(linspace(0,size(y,1)/Fs_RX,size(y,1)), y(:,i));
        xlabel('Time (s)');
        ylabel('Response');
        hold on;
        plot(t(i), 0, 'r.', 'MarkerSize', 10);
        hold off;
        subplot(size(y,2),2,i*2);
        LinePlotReducer(linspace(0,size(hi,2)/Fs_RX,size(hi,2)), hi);
        xlabel('Time (s)');
        ylabel('Response');
    end
end

% Calculate the matched filter result for one mic channel
function [h, sample] = single_matched_filter(X, y)
    h = y' * X';
    
    % Find the peak
    max = 0;
    max_val = 0;
    for i = 1:length(h)
       if abs(h(i)) > max_val
           max = i;
           max_val = abs(h(i));
       end
    end
    sample = max;
end